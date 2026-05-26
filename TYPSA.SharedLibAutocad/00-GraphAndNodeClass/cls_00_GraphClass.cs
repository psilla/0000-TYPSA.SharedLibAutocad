using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;
using static TYPSA.SharedLib.Autocad.GetEntities.cls_00_NodeClass;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GraphClass
    {
        public class Graph
        {

            public void RemoveEntity(Entity ent)
            {
                foreach (var node in AdjacencyList.Keys.ToList())
                {
                    AdjacencyList[node] = AdjacencyList[node]
                        .Where(t => t.Item2 != ent)
                        .ToList();
                }
            }

            // Vecino + Entidad conectora
            public Dictionary<cls_00_NodeClass.NodePoint, List<Tuple<cls_00_NodeClass.NodePoint, Entity>>> AdjacencyList { get; } =
                new Dictionary<cls_00_NodeClass.NodePoint, List<Tuple<cls_00_NodeClass.NodePoint, Entity>>>();

            public Entity GetEntityOfNode(
                cls_00_NodeClass.NodePoint node
            )
            {
                if (!AdjacencyList.ContainsKey(node))
                    return null;

                // Tomamos la primera conexión del nodo y extraemos la entidad asociada
                var connections = AdjacencyList[node];
                if (connections.Count > 0)
                    return connections.First().Item2;

                return null;
            }

            public void AddEdge(
                Point3d a, 
                Point3d b, 
                Entity ent
            )
            {
                cls_00_NodeClass.NodePoint na = FindOrAddNode(a);
                cls_00_NodeClass.NodePoint nb = FindOrAddNode(b);

                if (!AdjacencyList.ContainsKey(na))
                    AdjacencyList[na] = new List<Tuple<cls_00_NodeClass.NodePoint, Entity>>();

                if (!AdjacencyList.ContainsKey(nb))
                    AdjacencyList[nb] = new List<Tuple<cls_00_NodeClass.NodePoint, Entity>>();

                // Agregar si no existe ya la conexión
                if (!AdjacencyList[na].Any(t => t.Item1.Equals(nb)))
                    AdjacencyList[na].Add(Tuple.Create(nb, ent));

                if (!AdjacencyList[nb].Any(t => t.Item1.Equals(na)))
                    AdjacencyList[nb].Add(Tuple.Create(na, ent));
            }

            public cls_00_NodeClass.NodePoint FindOrAddNode(
                Point3d pt
            )
            {
                foreach (var key in AdjacencyList.Keys)
                {
                    if (key.Point.IsEqualTo(pt, new Tolerance(0.01, 0.01)))
                        return key;
                }

                cls_00_NodeClass.NodePoint newNode = new cls_00_NodeClass.NodePoint(pt);
                AdjacencyList[newNode] = new List<Tuple<cls_00_NodeClass.NodePoint, Entity>>();
                return newNode;
            }

            public cls_00_NodeClass.NodePoint FindClosestNode(
                Point3d pt, 
                double tolerance = 0.5
            )
            {
                double minDist = double.MaxValue;
                cls_00_NodeClass.NodePoint closest = null;

                foreach (var node in AdjacencyList.Keys)
                {
                    double dist = pt.DistanceTo(node.Point);
                    if (dist < minDist && dist <= tolerance)
                    {
                        minDist = dist;
                        closest = node;
                    }
                }

                return closest;
            }

            public List<cls_00_NodeClass.NodePoint> GetAllPointsFromEntity(
                Entity ent
            )
            {
                return AdjacencyList
                    .SelectMany(kvp => kvp.Value
                        .Where(t => t.Item2 == ent)
                        .Select(t => t.Item1))
                    .Distinct()
                    .ToList();
            }

            public List<cls_00_NodeClass.NodePoint> OrderedPointsOnEntity(
                Entity entity
            )
            {
                // Obtener todos los puntos del grafo que pertenecen a esta entidad
                var pointsOnEntity = AdjacencyList
                    .Where(kvp => kvp.Value.Any(t => t.Item2 == entity))
                    .Select(kvp => kvp.Key)
                    .Distinct()
                    .ToList();

                if (pointsOnEntity.Count <= 2)
                    return pointsOnEntity;

                // Empezamos desde uno de los extremos: aquel con solo un vecino en la entidad
                cls_00_NodeClass.NodePoint start = pointsOnEntity
                    .FirstOrDefault(p =>
                        AdjacencyList[p].Count(n => n.Item2 == entity) == 1);

                if (start == null)
                    start = pointsOnEntity.First(); // fallback, si es un ciclo cerrado

                var ordered = new List<cls_00_NodeClass.NodePoint> { start };
                var visited = new HashSet<cls_00_NodeClass.NodePoint> { start };
                cls_00_NodeClass.NodePoint current = start;

                while (true)
                {
                    var next = AdjacencyList[current]
                        .Where(t => t.Item2 == entity && !visited.Contains(t.Item1))
                        .Select(t => t.Item1)
                        .FirstOrDefault();

                    if (next == null)
                        break;

                    ordered.Add(next);
                    visited.Add(next);
                    current = next;
                }

                return ordered;
            }

            public Entity FindEntityConnectingPoints(
                Point3d p1, 
                Point3d p2
            )
            {
                foreach (var kvp in AdjacencyList)
                {
                    foreach (var (neighbor, entity) in kvp.Value)
                    {
                        if ((kvp.Key.Point.IsEqualTo(p1, Tolerance.Global) && neighbor.Point.IsEqualTo(p2, Tolerance.Global)) ||
                            (kvp.Key.Point.IsEqualTo(p2, Tolerance.Global) && neighbor.Point.IsEqualTo(p1, Tolerance.Global)))
                        {
                            return entity;
                        }
                    }
                }
                return null;
            }

            public bool FindShortestPathInEntity(
                cls_00_NodeClass.NodePoint start, 
                cls_00_NodeClass.NodePoint goal, 
                Entity entity, 
                out List<cls_00_NodeClass.NodePoint> path
            )
            {
                path = new List<cls_00_NodeClass.NodePoint>();

                // Caso trivial: son iguales
                if (start.Equals(goal))
                {
                    path.Add(start);
                    return true;
                }

                // Caso directo: están conectados por la misma entidad
                if (AdjacencyList[start].Any(t => t.Item1.Equals(goal) && t.Item2 == entity))
                {
                    path.Add(start);
                    path.Add(goal);
                    return true;
                }

                // BFS limitado a la entidad
                var cameFrom = new Dictionary<cls_00_NodeClass.NodePoint, cls_00_NodeClass.NodePoint>();
                var queue = new Queue<cls_00_NodeClass.NodePoint>();
                var visited = new HashSet<cls_00_NodeClass.NodePoint>();

                queue.Enqueue(start);
                visited.Add(start);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    foreach (var (neighbor, ent) in AdjacencyList[current])
                    {
                        if (ent != entity) continue;
                        if (visited.Contains(neighbor)) continue;

                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        cameFrom[neighbor] = current;

                        if (neighbor.Equals(goal))
                            break; // Terminamos apenas llegamos al goal
                    }
                }

                if (!cameFrom.ContainsKey(goal))
                    return false;

                // Reconstrucción del camino
                var temp = goal;
                path.Add(temp);

                while (cameFrom.ContainsKey(temp))
                {
                    temp = cameFrom[temp];
                    path.Insert(0, temp);
                }

                return true;
            }

            public class cls_07_LineComparer : IEqualityComparer<Line>
            {
                public bool Equals(Line l1, Line l2)
                {
                    if (l1 == null || l2 == null) return false;

                    return (l1.StartPoint.IsEqualTo(l2.StartPoint, Tolerance.Global) &&
                            l1.EndPoint.IsEqualTo(l2.EndPoint, Tolerance.Global)) ||
                           (l1.StartPoint.IsEqualTo(l2.EndPoint, Tolerance.Global) &&
                            l1.EndPoint.IsEqualTo(l2.StartPoint, Tolerance.Global));
                }

                public int GetHashCode(Line line)
                {
                    unchecked
                    {
                        var hash1 = line.StartPoint.GetHashCode();
                        var hash2 = line.EndPoint.GetHashCode();
                        return hash1 ^ hash2;
                    }
                }
            }


            public List<Line> GetLinesFromEntity(
                Entity entity
            )
            {
                return AdjacencyList
                    .SelectMany(kvp => kvp.Value
                        .Where(t => t.Item2 == entity)
                        .Select(t =>
                        {
                            var p1 = kvp.Key.Point;
                            var p2 = t.Item1.Point;
                            return new Line(p1, p2);
                        }))
                    .Distinct(new cls_07_LineComparer())
                    .ToList();
            }
        }

        public static void DrawAllPointsFromGraphNodes(
            Graph graph,
            Transaction tr,
            BlockTableRecord btr,
            short colorIndex = 1,
            string layerName = "0"
        )
        {
            // Recorremos los nodos del grafo
            foreach (var node in graph.AdjacencyList.Keys)
            {
                // Obtenemos el pto a partir del nodo
                DBPoint pt = new DBPoint(node.Point);

                // Asignamos la capa
                pt.Layer = layerName;

                // Asignamos el color
                pt.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                    Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorIndex); // Rojo

                // Agregar a la BlockTableRecord
                cls_00_DocumentInfo.AddEntityToBlockTableRecord(pt, btr, tr);
            }
        }

        


    }
}
