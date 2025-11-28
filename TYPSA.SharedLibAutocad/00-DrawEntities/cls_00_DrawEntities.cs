using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawEntities
    {

        public static double GetPolylineLength(
            List<Point3d> path
        )
        {
            // Definimos la variable
            double totalLength = 0.0;
            // Por cada segmento
            for (int i = 1; i < path.Count; i++)
            {
                // Sumamos la longitud
                totalLength += path[i - 1].DistanceTo(path[i]);
            }
            // return
            return totalLength;
        }

        public static Dictionary<int, string> GetAciColorNames()
        {
            var aciColorNames = new Dictionary<int, string>();

            for (int i = 0; i <= 256; i++)
            {
                string name;

                switch (i)
                {
                    case 0:
                        name = "ByBlock";
                        break;
                    case 1:
                        name = "Red";
                        break;
                    case 2:
                        name = "Yellow";
                        break;
                    case 3:
                        name = "Green";
                        break;
                    case 4:
                        name = "Cyan";
                        break;
                    case 5:
                        name = "Blue";
                        break;
                    case 6:
                        name = "Magenta";
                        break;
                    case 7:
                        name = "White / Black";
                        break;
                    case 8:
                        name = "Dark Gray";
                        break;
                    case 9:
                        name = "Light Gray";
                        break;
                    case 256:
                        name = "ByLayer";
                        break;
                    default:
                        if (i >= 250 && i <= 254)
                            name = $"Very Light Gray {i}";
                        else if (i == 255)
                            name = "Almost White 255";
                        else
                            name = $"Color {i}";
                        break;
                }

                aciColorNames[i] = name;
            }

            return aciColorNames;
        }

        public static AttachmentPoint AskMTextJustificationFromUser(
            AttachmentPoint defaultJustification = AttachmentPoint.TopLeft
        )
        {
            //// Lista de opciones (nombre → enum)
            //Dictionary<string, AttachmentPoint> justifications = Enum
            //    .GetValues(typeof(AttachmentPoint))
            //    .Cast<AttachmentPoint>()
            //    .ToDictionary(j => j.ToString(), j => j);

            // Valores validos para MText
            AttachmentPoint[] validValues = new[]
            {
                AttachmentPoint.TopLeft, AttachmentPoint.TopCenter, AttachmentPoint.TopRight,
                AttachmentPoint.MiddleLeft, AttachmentPoint.MiddleCenter, AttachmentPoint.MiddleRight,
                AttachmentPoint.BottomLeft, AttachmentPoint.BottomCenter, AttachmentPoint.BottomRight
            };

            // Diccionario de opciones
            Dictionary<string, AttachmentPoint> justifications = validValues
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



