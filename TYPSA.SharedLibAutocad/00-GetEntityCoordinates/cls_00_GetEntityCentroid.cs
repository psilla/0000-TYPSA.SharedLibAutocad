using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.GetEntityCoordinates
{
    public class cls_00_GetEntityCentroid
    {
        public static Point3d GetEntityCentroid(Entity entity)
        {
            Extents3d extents = entity.GeometricExtents;
            double centerX = (extents.MinPoint.X + extents.MaxPoint.X) / 2.0;
            double centerY = (extents.MinPoint.Y + extents.MaxPoint.Y) / 2.0;
            double centerZ = (extents.MinPoint.Z + extents.MaxPoint.Z) / 2.0;
            return new Point3d(centerX, centerY, centerZ);
        }

        // Comparador para ordenar regiones por fila (Y → X)
        public static int CompareEntitiesByPosition(Entity a, Entity b, double yTolerance = 5.0)
        {
            Point3d ca = GetEntityCentroid(a);
            Point3d cb = GetEntityCentroid(b);

            // Primero comparamos Y (arriba a abajo)
            double diffY = cb.Y - ca.Y; // descendente (mayor Y primero)
            if (Math.Abs(diffY) > yTolerance)
            {
                return diffY > 0 ? 1 : -1;
            }

            // Si están en la misma "fila", ordenar por X (izquierda a derecha)
            return ca.X.CompareTo(cb.X);
        }


        public static int CompareEntitiesByPositionHorizontal(Entity a, Entity b, double xTolerance = 0.001)
        {
            Point3d ca = GetEntityCentroid(a);
            Point3d cb = GetEntityCentroid(b);

            // Primero comparamos X (izquierda a derecha)
            double diffX = ca.X - cb.X;
            if (Math.Abs(diffX) > xTolerance)
            {
                return diffX < 0 ? -1 : 1;
            }

            // Si están en la misma columna, ordenar por Y descendente (arriba primero)
            double diffY = cb.Y - ca.Y;
            if (Math.Abs(diffY) > xTolerance)
            {
                return diffY > 0 ? 1 : -1;
            }

            return 0;
        }
        public static List<Entity> OrderByColumns(
            IEnumerable<Entity> entities,
            Func<Entity, Point3d> getCentroid,
            double xTolerance = 1.0)
        {
            if (entities == null) return new List<Entity>();

            // 1) Ordenamos por X ascendente (izq -> der)
            var ordered = entities
                .Select(e => new { Ent = e, C = getCentroid(e) })
                .OrderBy(x => x.C.X)
                .ToList();

            var result = new List<Entity>();
            var currentColumn = new List<(Entity Ent, Point3d C)>();
            double? refX = null;

            // 2) Agrupar en columnas por tolerancia en X
            foreach (var item in ordered)
            {
                if (refX == null || Math.Abs(item.C.X - refX.Value) <= xTolerance)
                {
                    currentColumn.Add((item.Ent, item.C));
                    refX = refX ?? item.C.X;
                }
                else
                {
                    // Ordenar columna actual por Y descendente (arriba -> abajo)
                    result.AddRange(currentColumn.OrderByDescending(x => x.C.Y).Select(x => x.Ent));

                    // Reiniciar columna
                    currentColumn = new List<(Entity, Point3d)> { (item.Ent, item.C) };
                    refX = item.C.X;
                }
            }

            // 3) Añadir la última columna
            if (currentColumn.Count > 0)
            {
                result.AddRange(currentColumn.OrderByDescending(x => x.C.Y).Select(x => x.Ent));
            }

            return result;
        }












    }
}
