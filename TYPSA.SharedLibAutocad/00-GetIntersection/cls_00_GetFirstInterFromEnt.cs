using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.EntitiesInsertionPoint;
using TYPSA.SharedLib.Autocad.GetEntities;
using static TYPSA.SharedLib.Autocad.GetEntities.cls_00_GetGraphFromLayers;
using static TYPSA.SharedLib.Autocad.GetEntities.cls_00_NodeClass;
using TYPSA.SharedLib.Autocad.DrawEntities;

namespace TYPSA.SharedLib.Autocad.GetIntersection
{
    public class cls_00_GetFirstInterFromEnt
    {
        public static (Point3d? ClosestPoint, Entity IntersectedEntity) GetFirstInterFromEnt(
            Point3d origin,
            Point3d goal,
            List<Line> clonedEntities
        )
        {
            // Creamos la vertical
            Line vertical = new Line(origin, goal);

            // Definimos variables por defecto
            Point3d? closestIntersection = null;
            double minDist = double.MaxValue;
            Entity intersectedEntity = null;

            // Contadores
            int totalCandidates = 0;
            int intersectedCount = 0;
            int noIntersectionCount = 0;

            // Recorremos entidades candidatas a ser intersectadas por la vertical
            foreach (Entity candidate in clonedEntities)
            {
                // Sumamos
                totalCandidates++;

                Point3dCollection pts = new Point3dCollection();
                // Vemos qué entidades intersectan con la vertical
                try
                {
                    candidate.IntersectWith(vertical, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                }
                catch (System.Exception ex)
                {
                    // continuamos
                    continue;
                }
                // Validamos 
                if (pts.Count == 0)
                {
                    // Sumamos
                    noIntersectionCount++;

                    // Mostramos
                    //sb.AppendLine($"🔸 Candidate #{totalCandidates}: No intersection.");
                    // continuamos
                    continue;
                }

                // Sumamos
                intersectedCount++;

                // Recorremos los ptos intersectados
                foreach (Point3d pt in pts)
                {
                    // Obtenemos distancias del pto del disconnect a cada punto de interseccion
                    double dist = origin.DistanceTo(pt);

                    // La primera interseccion es la elegida
                    if (dist < minDist)
                    {
                        // Asignamos
                        minDist = dist;
                        closestIntersection = pt;
                        intersectedEntity = candidate;
                    }
                }
            }

            // return
            return (closestIntersection, intersectedEntity);
        }

        public static (
            Point3d? ClosestPoint,
            Entity IntersectedEntity,
            bool NoCapacity,
            string FullMethod
        ) GetFirstInterFromEnt_AZTEC(
            Point3d origin,
            Point3d goal,
            List<GraphLine> clonedEntities,
            Dictionary<string, int> maxCircuitsByMethod,
            Dictionary<string, int> circuitsUsedByEntity
        )
        {
            // Definimos vertical
            Line vertical = new Line(origin, goal);

            // Definimos variables por defecto
            Point3d? closestIntersection = null;
            double minDist = double.MaxValue;
            Entity intersectedEntity = null;
            bool noCapacityFound = false;
            string fullMethod = null;

            // Iteramos
            foreach (GraphLine candidate in clonedEntities)
            {
                // Obtenemos info
                Line line = candidate.Segment;
                string method = candidate.InstallationMethod;

                Point3dCollection pts = new Point3dCollection();
                // Vemos qué entidades intersectan con la vertical
                try
                {
                    line.IntersectWith(vertical, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                }
                catch
                {
                    // continuamos
                    continue;
                }
                // Validamos
                if (pts.Count == 0) continue;

                // ahora validamos capacidad
                if (!string.IsNullOrEmpty(method) && maxCircuitsByMethod.TryGetValue(method, out int maxCircuits))
                {
                    // Contamos circuitos usados en la entidad
                    int used = cls_00_GetGraphFromLayers.GetUsedCircuitsForOrigEntByGraph(
                        candidate, circuitsUsedByEntity
                    );
                    bool hasCapacity = used < maxCircuits;

                    // Validamos
                    if (!hasCapacity)
                    {
                        noCapacityFound = true;
                        fullMethod = method;
                        continue;
                    }
                }

                // Recorremos los ptos intersectados
                foreach (Point3d pt in pts)
                {
                    // Obtenemos distancias del pto del disconnect a cada punto de interseccion
                    double dist = origin.DistanceTo(pt);
                    // La primera interseccion es la elegida
                    if (dist < minDist)
                    {
                        // Asignamos
                        minDist = dist;
                        closestIntersection = pt;
                        intersectedEntity = line;
                    }
                }
            }

            // return
            return (closestIntersection, intersectedEntity, noCapacityFound, fullMethod);
        }








    }
}
