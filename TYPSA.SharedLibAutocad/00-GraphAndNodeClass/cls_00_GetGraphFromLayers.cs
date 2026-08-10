using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetLayersInfo;
using TYPSA.SharedLib.Autocad.ProcessPoly;
using static TYPSA.SharedLib.Autocad.GetEntities.cls_00_NodeClass;
using TYPSA.SharedLib.Autocad.DrawEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class GraphSegment
    {
        public GraphLine GraphLine;
        public Point3d Start;
        public Point3d End;

        public override bool Equals(object obj)
        {
            var other = obj as GraphSegment;
            if (other == null) return false;

            if (GraphLine.OriginalEntityId != other.GraphLine.OriginalEntityId)
                return false;

            var s1 = cls_00_DrawPoint.NormalizePointWithNoTol(Start);
            var e1 = cls_00_DrawPoint.NormalizePointWithNoTol(End);
            var s2 = cls_00_DrawPoint.NormalizePointWithNoTol(other.Start);
            var e2 = cls_00_DrawPoint.NormalizePointWithNoTol(other.End);

            bool sameDirection = s1 == s2 && e1 == e2;
            bool oppositeDirection = s1 == e2 && e1 == s2;

            return sameDirection || oppositeDirection;
        }

        public override int GetHashCode()
        {
            var s = cls_00_DrawPoint.NormalizePointWithNoTol(Start);
            var e = cls_00_DrawPoint.NormalizePointWithNoTol(End);

            // Ordenamos los puntos para que (A→B) == (B→A)
            var p1 = s.CompareTo(e) <= 0 ? s : e;
            var p2 = s.CompareTo(e) <= 0 ? e : s;

            int hash = 17;
            hash = hash * 23 + GraphLine.OriginalEntityId.GetHashCode();
            hash = hash * 23 + p1.GetHashCode();
            hash = hash * 23 + p2.GetHashCode();

            return hash;
        }
    }

    public class SegmentInfo
    {
        public Line Entity { get; set; }
        public Point3d Start { get; set; }
        public Point3d End { get; set; }
        public string EnergyStr { get; set; }
        public string Layer { get; set; }
        public List<string> DcBlockLabels { get; set; } = new List<string>();

        // Info en Pset
        public double Energy { get; set; }
        public int Circuits { get; set; }
        public double Length { get; set; }
        public bool Compliance { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public int MaxCircuitsAllowed { get; set; }
    }

    public class GraphLine
    {
        public Line Segment { get; set; }
        public string OriginalLayer { get; set; }
        public ObjectId OriginalEntityId { get; set; }
        public string InstallationMethod { get; set; }
        public string InstallationMethodType { get; set; }
        public Point3d Start { get; set; }
        public Point3d End { get; set; }
        public int MaxNumCircuitsByType { get; set; }
    }

    public class cls_00_GetGraphFromLayers
    {
        
        private static bool IsPointBetween(Point3d p, Point3d a, Point3d b, double tol = 0.01)
        {
            double distAB = a.DistanceTo(b);
            double distAP = a.DistanceTo(p);
            double distPB = p.DistanceTo(b);

            return Math.Abs((distAP + distPB) - distAB) < tol;
        }

        public static List<GraphSegment> GetSubSegmentsFromEntityByPoints(
            NodePoint currentNode,
            NodePoint candNode,
            cls_00_GraphClass.Graph graph,
            GraphLine currentGraphLine
        )
        {
            // -----------------------------
            // Obtener los nodos del grafo en esa entidad conectados a otra
            // -----------------------------

            List<NodePoint> allPoints = graph.GetAllPointsFromEntity(currentGraphLine.Segment)
                .Where(p =>
                    graph.AdjacencyList.ContainsKey(p) &&
                    graph.AdjacencyList[p].Any(adj => adj.Item2 != currentGraphLine.Segment)
                )
                .ToList();
            // Validamos
            if (allPoints == null || allPoints.Count == 0) return new List<GraphSegment>();

            // -----------------------------
            // Filtrar nodos entre start y end
            // -----------------------------

            List<NodePoint> between = allPoints
                .Where(p => IsPointBetween(p.Point, currentNode.Point, candNode.Point))
                .OrderBy(p => p.Point.DistanceTo(currentNode.Point))
                .ToList();

            // -----------------------------
            // Incluir start/end si por tolerancia no entran
            // -----------------------------

            if (!between.Any(p => p.Equals(currentNode)))
                between.Insert(0, currentNode);

            if (!between.Any(p => p.Equals(candNode)))
                between.Add(candNode);

            // -----------------------------
            // Crear subsegmentos 
            // -----------------------------

            List<GraphSegment> subSegments = new List<GraphSegment>();

            // -----------------------------
            // Comprobar si hay ptos intermedios 
            // -----------------------------

            bool hasIntermediatePoints = between.Count > 2;

            // -----------------------------
            // Caso con ptos intermedios
            // -----------------------------

            if (hasIntermediatePoints)
            {
                // Iteramos
                for (int i = 0; i < between.Count - 1; i++)
                {
                    // Añadimos
                    subSegments.Add(new GraphSegment
                    {
                        GraphLine = currentGraphLine,
                        Start = (between[i].Point),
                        End = (between[i + 1].Point)
                    });
                }
            }

            // -----------------------------
            // Caso sin ptos intermedios
            // -----------------------------

            else
            {
                subSegments.Add(new GraphSegment
                {
                    GraphLine = currentGraphLine,
                    Start = (currentNode.Point),
                    End = (candNode.Point),
                });
            }

            // return
            return subSegments;
        }

        public static int GetUsedCircuitsForOrigEntByGraph(
            GraphLine graphLine,
            Dictionary<string, int> circuitsUsedByOriginalEntity
        )
        {
            // Validamos
            if (graphLine == null) return 0;
            // Parseamos
            string key = graphLine.OriginalEntityId.Handle.ToString();
            // Obtenemos circuitos en uso
            circuitsUsedByOriginalEntity.TryGetValue(key, out int used);
            // return
            return used;
        }

        public static cls_00_GraphClass.Graph GetGraphFromLayers(
            Database db,
            Transaction tr,
            BlockTableRecord btr,
            HashSet<string> layersByDefault,
            string layersFormMess,
            out List<Line> clonedEntities,
            out Dictionary<ObjectId, Entity> originalEntities,
            out HashSet<string> layersByUserSet,
            out Dictionary<ObjectId, List<Line>> dictPolyIdExplodedLines
        )
        {
            // Lista para almacenar las entidades clonadas
            clonedEntities = new List<Line>();
            // Creamos un diccionario de las entidades originales
            originalEntities = new Dictionary<ObjectId, Entity>();
            // HashSet de capas obtenidas
            layersByUserSet = new HashSet<string>();
            // Dict para relacionar poly original con lineas explotadas
            dictPolyIdExplodedLines = new Dictionary<ObjectId, List<Line>>();

            // Obtenemos las capas deseadas
            layersByUserSet = cls_00_GetSelLayersByDefaultFromDoc.
                GetSelLayersByDefaultFromDoc(layersByDefault, layersFormMess);
            // Validamos
            if (layersByUserSet == null) return null;

            int lineCount, polylineCount, arcCount;
            HashSet<string> capasUsadas;
            // Creamos enumerable de entidades a partir lineas/poly de las capas anteriores
            IEnumerable<Entity> entitiesFromLinesAndPoly =
                cls_00_GetPolyAndLinesByLayerFilterAsEnu.GetPolyAndLinesByLayerFilterAsEnu(
                    db, tr, btr, layersByUserSet,
                    out lineCount, out polylineCount, out arcCount,
                    out capasUsadas
                );
            // Validamos
            if (entitiesFromLinesAndPoly == null) return null;

            // Iteramos
            foreach (Entity ent in entitiesFromLinesAndPoly)
            {
                // Almacenamos
                originalEntities[ent.ObjectId] = ent;
            }

            // Iteramos por las entidades originales
            foreach (Entity ent in entitiesFromLinesAndPoly)
            {
                // Hacemos una copia de las entidades y las explotamos
                clonedEntities.AddRange(
                    cls_00_CloneAndExpPoly.CloneAndExplodePoly(ent, dictPolyIdExplodedLines)
                );
            }

            // Creamos el grafo con esas entidades clonadas y/o explotadas
            cls_00_GraphClass.Graph graph =
                cls_00_GraphTools.BuildGraphFromEntities(clonedEntities);

            // return
            return graph;
        }







    }
}
