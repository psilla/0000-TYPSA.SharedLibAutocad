using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawMtext
    {
        public static ObjectId DrawMTextOnPoint(
            Point3d tagPoint,
            string tagValue,
            Transaction tr,
            BlockTableRecord btr,
            bool horizontal = false, // false = vertical (por defecto)
            double textHeight = 7.2,
            int colorIndex = 1,
            string layer = "0",
            string textStyle = null, // Nuevo parámetro opcional
            AttachmentPoint? justification = null // Nuevo parámetro opcional
        )
        {
            // Crear el objeto MText
            MText mText = new MText
            {
                Location = tagPoint,
                Contents = tagValue,
                TextHeight = textHeight,
                Layer = layer,
                ColorIndex = colorIndex,
                Rotation = horizontal ? 0 : Math.PI / 2 // Orientación: horizontal o vertical
            };

            // Asignar justificación si se proporciona
            if (justification.HasValue)
            {
                mText.Attachment = justification.Value;
            }

            // Asignar estilo de texto si se proporciona
            if (!string.IsNullOrEmpty(textStyle))
            {
                TextStyleTable tst = (TextStyleTable)tr.GetObject(btr.Database.TextStyleTableId, OpenMode.ForRead);

                if (tst.Has(textStyle))
                {
                    mText.TextStyleId = tst[textStyle];
                }
            }

            // Insertar el MText en el dibujo
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(mText, btr, tr);

            // return
            return mText.ObjectId;
        }

        public static void DrawMultilineText(
            Transaction tr,
            BlockTableRecord btr,
            Extents3d ext,
            string msg,
            double textHeight = 1.5,
            double width = 60,
            int colorIndex = 1,
            string layer = "E-HOMERUN-FAILED",
            int maxLineLength = 30
        )
        {
            // Generar el texto multilínea con saltos de línea (\P para AutoCAD MText)
            List<string> wrappedLines = WrapText(msg, maxLineLength);
            string formatted = string.Join("\\P", wrappedLines);

            // Crear el MText
            MText mtext = new MText
            {
                Location = new Point3d(ext.MinPoint.X + 5, ext.MinPoint.Y + 5, ext.MinPoint.Z),
                TextHeight = textHeight,
                Contents = formatted,
                Layer = layer,
                ColorIndex = colorIndex,
                Width = width
            };

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(mtext, btr, tr);
        }

        private static List<string> WrapText(string input, int maxLineLength = 30)
        {
            var words = input.Split(' ');
            var result = new List<string>();
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > maxLineLength)
                {
                    result.Add(currentLine.ToString().TrimEnd());
                    currentLine.Clear();
                }

                currentLine.Append(word + " ");
            }

            if (currentLine.Length > 0)
                result.Add(currentLine.ToString().TrimEnd());

            return result;
        }



    }
}
