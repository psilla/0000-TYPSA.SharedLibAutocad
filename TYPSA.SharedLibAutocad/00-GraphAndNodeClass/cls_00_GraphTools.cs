using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GraphTools
    {
        public static cls_00_GraphClass.Graph BuildGraphFromEntities(
            IEnumerable<Entity> entities
        )
        {
            cls_00_GraphClass.Graph graph = new cls_00_GraphClass.Graph();

            foreach (var ent in entities)
            {
                try
                {
                    if (ent is Line line)
                    {
                        // Incluye la entidad
                        graph.AddEdge(line.StartPoint, line.EndPoint, line);
                    }
                    else if (ent is Polyline pline)
                    {
                        for (int i = 0; i < pline.NumberOfVertices - 1; i++)
                        {
                            Point3d p1 = pline.GetPoint3dAt(i);
                            Point3d p2 = pline.GetPoint3dAt(i + 1);
                            // Incluye la entidad
                            graph.AddEdge(p1, p2, pline);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(
                        $"⚠ Error building graph: {ex.Message}"
                    );
                }
            }
            // return
            return graph;
        }

        public static void InjectPointIntoGraph(
            Point3d pt,
            Entity ent,
            cls_00_GraphClass.Graph graph
        )
        {
            if (ent is Line line)
            {
                graph.AddEdge(line.StartPoint, pt, line);
                graph.AddEdge(pt, line.EndPoint, line);
            }
            else if (ent is Polyline pline)
            {
                int closestSegment = -1;
                double minDist = double.MaxValue;

                for (int i = 0; i < pline.NumberOfVertices - 1; i++)
                {
                    Point3d a = pline.GetPoint3dAt(i);
                    Point3d b = pline.GetPoint3dAt(i + 1);
                    Line temp = new Line(a, b);
                    double dist = temp.GetClosestPointTo(pt, false).DistanceTo(pt);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestSegment = i;
                    }
                }

                if (closestSegment >= 0)
                {
                    Point3d v1 = pline.GetPoint3dAt(closestSegment);
                    Point3d v2 = pline.GetPoint3dAt(closestSegment + 1);

                    graph.AddEdge(v1, pt, pline);
                    graph.AddEdge(pt, v2, pline);
                }
            }
        }

        public static void InjectEntityIntersectionsIntoGraph(
            IEnumerable<Entity> entities,
            cls_00_GraphClass.Graph graph
        )
        {
            var entityList = entities.ToList();
            var injectedPoints = new HashSet<Point3d>(new Point3dEqualityComparer(1e-6)); // para evitar duplicados

            for (int i = 0; i < entityList.Count - 1; i++)
            {
                for (int j = i + 1; j < entityList.Count; j++)
                {
                    Entity ent1 = entityList[i];
                    Entity ent2 = entityList[j];

                    Point3dCollection pts = new Point3dCollection();

                    try
                    {
                        ent1.IntersectWith(ent2, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                        foreach (Point3d pt in pts)
                        {
                            if (injectedPoints.Add(pt))
                            {
                                InjectPointIntoGraph(pt, ent1, graph);
                                InjectPointIntoGraph(pt, ent2, graph);
                            }
                        }
                    }
                    catch
                    {
                        // Si no pueden intersectar, seguimos
                        continue;
                    }

                    // Analizamos extremos coincidentes
                    var ends1 = GetEndPoints(ent1);
                    var ends2 = GetEndPoints(ent2);

                    foreach (var p1 in ends1)
                    {
                        foreach (var p2 in ends2)
                        {
                            if (p1.DistanceTo(p2) < 1e-6)
                            {
                                if (injectedPoints.Add(p1))
                                {
                                    InjectPointIntoGraph(p1, ent1, graph);
                                    InjectPointIntoGraph(p2, ent2, graph);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void InjectInverterPointsIntoGraph(
            List<Point3d> inverterPoints,
            List<Line> lines,
            cls_00_GraphClass.Graph graph
        )
        {
            foreach (var pt in inverterPoints)
            {
                foreach (var line in lines)
                {
                    // Verificamos si el punto está sobre la línea
                    if (IsPointOnLine(pt, line))
                    {
                        InjectPointIntoGraph(pt, line, graph);
                    }
                }
            }
        }

        public static bool IsPointOnLine(Point3d pt, Line line)
        {
            // Proyecta el punto sobre la línea
            Point3d closest = line.GetClosestPointTo(pt, false);

            // Si está muy cerca, asumimos que está sobre la línea
            return pt.DistanceTo(closest) < 1e-6;
        }


        // Función auxiliar para obtener extremos de entidad
        private static List<Point3d> GetEndPoints(Entity ent)
        {
            var pts = new List<Point3d>();

            if (ent is Line ln)
                pts.AddRange(new[] { ln.StartPoint, ln.EndPoint });

            else if (ent is Polyline pl)
                pts.AddRange(new[] { pl.StartPoint, pl.EndPoint });

            else if (ent is Polyline2d pl2d)
            {
                var verts = pl2d.Cast<Vertex2d>().ToList();
                if (verts.Count >= 2)
                    pts.AddRange(new[] { verts.First().Position, verts.Last().Position });
            }

            // return
            return pts;
        }

        // Comparador con tolerancia para evitar puntos duplicados
        private class Point3dEqualityComparer : IEqualityComparer<Point3d>
        {
            private readonly double _tolerance;

            public Point3dEqualityComparer(double tolerance)
            {
                _tolerance = tolerance;
            }

            public bool Equals(Point3d p1, Point3d p2)
            {
                return p1.DistanceTo(p2) < _tolerance;
            }

            public int GetHashCode(Point3d p)
            {
                // Redondeamos coordenadas a múltiplos de tolerancia para generar hash
                int hashX = (int)(p.X / _tolerance);
                int hashY = (int)(p.Y / _tolerance);
                int hashZ = (int)(p.Z / _tolerance);
                return hashX ^ hashY ^ hashZ;
            }
        }

        public static void RemoveIsolatedPointsNotSharedBetweenEntities(cls_00_GraphClass.Graph graph)
        {
            var pointsToRemove = new List<cls_00_NodeClass.NodePoint>();

            foreach (var kvp in graph.AdjacencyList)
            {
                cls_00_NodeClass.NodePoint p = kvp.Key;
                var neighbors = kvp.Value;

                // Obtenemos el conjunto de entidades distintas a las que está conectado el punto
                var connectedEntities = neighbors
                    .Select(n => n.Item2)
                    .Where(ent => ent != null)
                    .Distinct()
                    .ToList();

                // Si solo pertenece a una (o ninguna) entidad, se elimina
                if (connectedEntities.Count < 2)
                {
                    pointsToRemove.Add(p);
                }
            }

            // Eliminamos del grafo todos los puntos inválidos
            foreach (var p in pointsToRemove)
            {
                graph.AdjacencyList.Remove(p);

                // Además, lo eliminamos como vecino en otros nodos
                foreach (var kvp in graph.AdjacencyList)
                {
                    kvp.Value.RemoveAll(n => n.Item1.Equals(p));
                }
            }
        }

        public static void RemoveEntitiesWithSingleNode(
            cls_00_GraphClass.Graph graph
        )
        {
            // Diccionario: Entity -> lista de NodePoints que la referencian
            Dictionary<Entity, List<cls_00_NodeClass.NodePoint>> entityToNodes =
                new Dictionary<Entity, List<cls_00_NodeClass.NodePoint>>();

            // Construir mapeo entidad → nodos donde aparece
            foreach (var kvp in graph.AdjacencyList)
            {
                cls_00_NodeClass.NodePoint node = kvp.Key;
                foreach (var neighbor in kvp.Value)
                {
                    Entity ent = neighbor.Item2;
                    if (ent != null)
                    {
                        if (!entityToNodes.ContainsKey(ent))
                            entityToNodes[ent] = new List<cls_00_NodeClass.NodePoint>();
                        if (!entityToNodes[ent].Contains(node))
                            entityToNodes[ent].Add(node);
                    }
                }
            }

            // Identificar entidades con solo un punto
            HashSet<Entity> entitiesToRemove = new HashSet<Entity>(
                entityToNodes.Where(kvp => kvp.Value.Count <= 1)
                             .Select(kvp => kvp.Key)
            );

            // Eliminar del grafo todos los nodos conectados solo a esas entidades
            var nodesToRemove = new List<cls_00_NodeClass.NodePoint>();

            foreach (var kvp in graph.AdjacencyList)
            {
                var node = kvp.Key;
                var filtered = kvp.Value.Where(n => !entitiesToRemove.Contains(n.Item2)).ToList();

                // Si el nodo queda sin conexiones, se marca para eliminar
                if (filtered.Count == 0)
                {
                    nodesToRemove.Add(node);
                }
                else
                {
                    graph.AdjacencyList[node] = filtered;
                }
            }

            // Eliminar nodos sin conexiones
            foreach (var node in nodesToRemove)
            {
                graph.AdjacencyList.Remove(node);
            }
        }



    }
}
