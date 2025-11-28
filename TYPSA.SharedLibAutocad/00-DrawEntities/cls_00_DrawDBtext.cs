using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawDBtext
    {
        public static DBText DrawDbTextOnPoint(
            Point3d tagPoint,
            string tagValue,
            Transaction tr,
            BlockTableRecord btr,
            bool horizontal = false, // false = vertical (por defecto)
            double textHeight = 7.2,
            int colorIndex = 1,
            string layer = "0",
            string textStyle = null // Nuevo parámetro opcional
        )
        {
            // Crear texto con orientación configurable
            DBText dbText = new DBText
            {
                Position = tagPoint,
                TextString = tagValue,
                Height = textHeight,
                Layer = layer,
                ColorIndex = colorIndex,
                Rotation = horizontal ? 0 : Math.PI / 2 // 👉 rotación según orientación
            };

            // Asignar estilo si se especifica
            if (!string.IsNullOrEmpty(textStyle))
            {
                // Buscar el TextStyle en el dibujo
                TextStyleTable tst = (TextStyleTable)tr.GetObject(btr.Database.TextStyleTableId, OpenMode.ForRead);

                // En caso de existir
                if (tst.Has(textStyle))
                {
                    // Obtenerlo
                    dbText.TextStyleId = tst[textStyle];
                }
            }

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(dbText, btr, tr);

            // return
            return dbText;
        }

        public static DBText DrawDbTextOnMidLinePoint(
            Line line,
            string textValue,
            Transaction tr,
            BlockTableRecord btr,
            double textHeight = 1.2,
            int colorIndex = 1,
            string layer = "E-HOMERUN",
            string textUnit = "" // opcional
        )
        {
            // Calcular punto medio de la línea
            Point3d mid = new Point3d(
                (line.StartPoint.X + line.EndPoint.X) / 2,
                (line.StartPoint.Y + line.EndPoint.Y) / 2,
                (line.StartPoint.Z + line.EndPoint.Z) / 2
            );

            // Dirección de la línea (vector normalizado)
            Vector3d direction = line.EndPoint - line.StartPoint;

            // Rotación de la línea
            double angle = direction.AngleOnPlane(new Plane(Point3d.Origin, Vector3d.ZAxis));

            // Forzar lectura de izquierda a derecha
            if (direction.X < 0) angle += Math.PI;

            // Obtener vector perpendicular (hacia la izquierda o arriba visualmente)
            Vector3d offsetVector = direction.CrossProduct(Vector3d.ZAxis).GetNormal();

            // Si apunta hacia abajo (Y negativo), invertirlo
            if (offsetVector.Y < 0)
                offsetVector = offsetVector.Negate();

            // Calcular nueva posición desplazada
            Point3d displacedMid = mid + offsetVector.MultiplyBy(textHeight / 2);

            // Construir texto (si textUnit está vacío, no se añade espacio extra)
            string text = string.IsNullOrWhiteSpace(textUnit)
                ? $"{textValue:0.00}"
                : $"{textValue:0.00} {textUnit}";

            // Crear texto 
            DBText dbText = new DBText
            {
                Position = displacedMid,
                TextString = text,
                Height = textHeight,
                Layer = layer,
                ColorIndex = colorIndex,
                Rotation = angle
            };

            // Alineaciones
            dbText.HorizontalMode = TextHorizontalMode.TextCenter;
            dbText.VerticalMode = TextVerticalMode.TextVerticalMid;
            // Pto Alineacion
            dbText.AlignmentPoint = displacedMid;
            // Ajustar Alineacion
            dbText.AdjustAlignment(btr.Database);

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(dbText, btr, tr);

            // return
            return dbText;
        }


    }
}
