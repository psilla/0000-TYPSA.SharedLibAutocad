using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

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

            // Definimos el pto de intersección como nulo
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

                // Creamos una colección vacia de ptos
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

                // Verificamos que haya habido alguna intersección
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
                    // Obtenemos distancias del pto del disconnect a cada punto de intersección
                    double dist = origin.DistanceTo(pt);

                    // La distancia mínima, es decir, la primera intersección, es la elegida
                    if (dist < minDist)
                    {
                        minDist = dist;
                        // Redefinimos el pto de intersección con el pto encontrado
                        closestIntersection = pt;
                        intersectedEntity = candidate;
                    }
                }
            }

            // return
            return (closestIntersection, intersectedEntity);
        }



    }
}
