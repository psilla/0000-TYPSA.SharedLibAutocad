using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_IsIntersectCross
    {
        public static bool IsIntersectionCross(cls_00_GraphClass.Graph graph, cls_00_NodeClass.NodePoint node)
        {
            // Validamos
            if (!graph.AdjacencyList.ContainsKey(node)) return false;

            var neighbors = graph.AdjacencyList[node];
            // Validamos
            if (neighbors == null || neighbors.Count == 0) return false;

            // Agrupamos por entidades (puede haber duplicados)
            List<Entity> entities = neighbors.Select(n => n.Item2).Distinct().ToList();

            int entCount = entities.Count;
            // Validamos numero de entidades
            if (entCount < 2) return false;

            // Obtenemos info
            Point3d pt = node.Point;
            Tolerance tol = new Tolerance(1e-6, 1e-6);

            // Caso: 2 entidades
            if (entCount == 2)
            {
                int numWherePtIsEndpoint = entities.Count(ent => IsEndPointOfEntity(ent, pt, tol));
                // si no es extremo en ninguna → cruce
                return numWherePtIsEndpoint == 0;
            }
            // Caso: 3 entidades
            if (entCount == 3)
            {
                int numWherePtIsEndpoint = entities.Count(ent => IsEndPointOfEntity(ent, pt, tol));
                // si hay al menos una en la que no sea extremo → cruce
                return numWherePtIsEndpoint < 3;
            }
            // return
            return true;
        }

        private static bool IsEndPointOfEntity(Entity ent, Point3d pt, Tolerance tol)
        {
            if (ent is Line line)
            {
                return line.StartPoint.IsEqualTo(pt, tol) || line.EndPoint.IsEqualTo(pt, tol);
            }
            else if (ent is Polyline pline)
            {
                int n = pline.NumberOfVertices;
                return pline.GetPoint3dAt(0).IsEqualTo(pt, tol) ||
                       pline.GetPoint3dAt(n - 1).IsEqualTo(pt, tol);
            }
            return false;
        }


    }
}
