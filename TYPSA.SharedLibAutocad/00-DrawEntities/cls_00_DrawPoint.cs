using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;
using System;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawPoint
    {
        private const int Decimals = 3;

        private static double RoundValue(double v) => Math.Round(v, Decimals);

        public static (double X, double Y, double Z) NormalizePointWithNoTol(
            Point3d p
        )
        {
            return (RoundValue(p.X), RoundValue(p.Y), RoundValue(p.Z));
        }

        public static Point3d NormalizePointWithTol(Point3d p)
        {
            double tol = 0.01;
            return new Point3d(
                Math.Round(p.X / tol) * tol,
                Math.Round(p.Y / tol) * tol,
                Math.Round(p.Z / tol) * tol
            );
        }

        public static void DrawPoint(
            Point3d point,
            Transaction tr,
            BlockTableRecord btr,
            short colorIndex = 1,
            string layerName = "E-HOMERUN"
        )
        {
            // Definimos el pto
            DBPoint dbPoint = new DBPoint(point);

            // Asignamos la capa
            dbPoint.Layer = layerName;

            // Asignamos el color 
            dbPoint.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorIndex);

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(dbPoint, btr, tr);
        }


    }
}
