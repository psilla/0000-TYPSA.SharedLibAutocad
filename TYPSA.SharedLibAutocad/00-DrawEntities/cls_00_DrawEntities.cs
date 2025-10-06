using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetDocument;
using TYPSA.SharedLib.Autocad.UserForms;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawEntities
    {
        public static void DrawMTextOnPoint(
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
        }

        public static AttachmentPoint AskMTextJustificationFromUser(
            AttachmentPoint defaultJustification = AttachmentPoint.TopLeft
        )
        {
            // Lista de opciones (nombre → enum)
            Dictionary<string, AttachmentPoint> justifications = Enum
                .GetValues(typeof(AttachmentPoint))
                .Cast<AttachmentPoint>()
                .ToDictionary(j => j.ToString(), j => j);

            // Form
            string selected = InstanciarFormularios.DropDownFormListOut(
                $"Select the justification for the labels:",
                justifications.Keys.OrderBy(k => k).ToList(),
                "Selection form to choose a text Justification",
                defaultJustification.ToString()
            );
            // Validamos
            if (string.IsNullOrEmpty(selected))
            {
                // Mensaje
                MessageBox.Show("⚠ No justification was selected. Operation cancelled.", "Warning");
                // return por defecto
                return defaultJustification;
            }
            // return
            return justifications[selected];
        }

        public static string AskTextStyleFromUser(
            List<string> textStyles,
            string textStyleByDefault = null
        )
        {
            // Mostramos formulario para elegir el estilo de texto
            string textStyle = InstanciarFormularios.DropDownFormListOut(
                $"Select the text style to use for the labels:",
                textStyles.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList(),
                "Selection form to choose a text Style",
                textStyleByDefault
            );
            // Validamos
            if (string.IsNullOrEmpty(textStyle))
            {
                // Mensaje
                MessageBox.Show("⚠ No text style was selected. Operation cancelled.", "Warning");
                // Finalizamos
                return null;
            }
            // return
            return textStyle;
        }














    }
}



