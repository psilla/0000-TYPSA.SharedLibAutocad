using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetDocument;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawEntities
    {

        public static ObjectId DrawMLeaderOnPoint(
            Point3d basePoint,
            string textValue,
            Transaction tr,
            BlockTableRecord btr,
            double textHeight = 7.2,
            int colorIndex = 1,
            string layer = "0",
            string textStyle = null,
            double offsetX = 2.0,
            double offsetY = 2.0,
            AttachmentPoint? justification = null
        )
        {
            // Crear el MLeader
            MLeader mLeader = new MLeader();
            mLeader.SetDatabaseDefaults();
            mLeader.Layer = layer;
            mLeader.ColorIndex = colorIndex;
            mLeader.ContentType = ContentType.MTextContent;

            // Crear el texto (MText)
            Point3d textPosition = new Point3d(basePoint.X + offsetX, basePoint.Y + offsetY, basePoint.Z);
            MText mText = new MText
            {
                Contents = textValue,
                TextHeight = textHeight,
                Location = textPosition,
                Attachment = justification ?? AttachmentPoint.MiddleLeft
            };

            // Aplicar estilo de texto si se especifica
            if (!string.IsNullOrEmpty(textStyle))
            {
                TextStyleTable tst = (TextStyleTable)tr.GetObject(btr.Database.TextStyleTableId, OpenMode.ForRead);
                if (tst.Has(textStyle))
                    mText.TextStyleId = tst[textStyle];
            }

            // Asignar el MText al MLeader
            mLeader.MText = mText;

            // Crear el líder (flecha)
            int leaderIndex = mLeader.AddLeader();
            int leaderLineIndex = mLeader.AddLeaderLine(leaderIndex);

            // Añadir el punto de inicio del líder
            mLeader.AddFirstVertex(leaderLineIndex, basePoint);

            // Añadir el punto de anclaje del texto (para que no apunte a 0,0,0)
            mLeader.AddLastVertex(leaderLineIndex, textPosition);

            // Asegurar que la dirección del texto se vincule correctamente
            mLeader.SetDoglegLength(leaderIndex, 5.0); // Ladding distance
            mLeader.LandingGap = 3.0;
            mLeader.EnableFrameText = true; // activa el recuadro

            // Insertar en el dibujo
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(mLeader, btr, tr);
            // return
            return mLeader.ObjectId;
        }

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

        public static Polyline DrawSegmentAsPolyline(
            List<Point3d> points,
            Transaction tr,
            BlockTableRecord btr,
            string layerName = "E-HOMERUN",
            short colorIndex = 4
        )
        {
            // Verificar que por lo menos tenemos 2 puntos
            if (points == null || points.Count < 2)
                // Finalizamos
                return null;

            // Try/Catch
            try
            {
                // Creamos la poly
                Polyline poly = new Polyline();

                // Añadimos los puntos de la lista
                for (int i = 0; i < points.Count; i++)
                    poly.AddVertexAt(i, new Point2d(points[i].X, points[i].Y), 0, 0, 0);

                // Asignamos la capa
                poly.Layer = layerName;

                // Asignamos el color
                poly.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                    Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorIndex); // Cyan

                // Agregar a la BlockTableRecord
                cls_00_DocumentInfo.AddEntityToBlockTableRecord(poly, btr, tr);

                // Devolver la polilínea creada
                return poly;
            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(
                    $"❌ Error al dibujar la polilínea: {ex.Message}"
                );

                // Finalizamos
                return null;
            }
        }

        public static Line DrawLineBetweenPoints(
            Point3d startPoint,
            Point3d endPoint,
            Transaction tr,
            BlockTableRecord btr,
            string layerName = "E-HOMERUN",
            short colorIndex = 2
        )
        {
            // Verificamos que los puntos no sean iguales
            if (startPoint == endPoint) return null;

            // try
            try
            {
                // Creamos la línea
                Line line = new Line(startPoint, endPoint);

                // Asignamos la capa
                line.Layer = layerName;

                // Asignamos el color
                line.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                    Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorIndex);

                // Agregar a la BlockTableRecord
                cls_00_DocumentInfo.AddEntityToBlockTableRecord(line, btr, tr);

                // return
                return line;
            }
            // catch
            catch (System.Exception ex)
            {
                // Mensaje
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(
                    $"❌ Error al dibujar la línea: {ex.Message}"
                );

                // Finalizamos
                return null;
            }
        }

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

        public static void DrawCircle(
            Transaction tr,
            BlockTableRecord btr,
            Point3d center,
            double radius = 3.0,
            string layerName = "E-HOMERUN-FAILED",
            short colorIndex = 1
        )
        {
            // Creamos el circulo
            using (var circle = new Circle(center, Vector3d.ZAxis, radius))
            {
                // Asignamos la capa
                circle.Layer = layerName;

                // Asignamos el color 
                circle.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                    Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorIndex);

                // Agregar a la BlockTableRecord
                cls_00_DocumentInfo.AddEntityToBlockTableRecord(circle, btr, tr);
            }
        }

        public static void DrawRevisionCloud(
            Transaction tr,
            BlockTableRecord btr,
            Point3d minPoint,
            Point3d maxPoint,
            double arcSpacing = 5.0,
            double bulgeFactor = 0.4,
            string layerName = "E-HOMERUN-FAILED",
            short colorIndex = 1
        )
        {
            // Coordenadas
            double xMin = minPoint.X;
            double yMin = minPoint.Y;
            double xMax = maxPoint.X;
            double yMax = maxPoint.Y;

            // Direcciones
            var corners = new List<(Point2d Start, Point2d End)>
            {
                (new Point2d(xMin, yMin), new Point2d(xMax, yMin)), // abajo
                (new Point2d(xMax, yMin), new Point2d(xMax, yMax)), // derecha
                (new Point2d(xMax, yMax), new Point2d(xMin, yMax)), // arriba
                (new Point2d(xMin, yMax), new Point2d(xMin, yMin))  // izquierda
            };

            Polyline pl = new Polyline();
            int vertexIndex = 0;

            foreach (var (start, end) in corners)
            {
                Vector2d dir = end - start;
                double length = dir.Length;
                int segments = Math.Max(2, (int)(length / arcSpacing));
                Vector2d step = dir / segments;

                for (int i = 0; i < segments; i++)
                {
                    Point2d pt = start + step * i;
                    double bulge = (i % 2 == 0) ? bulgeFactor : -bulgeFactor;
                    pl.AddVertexAt(vertexIndex++, pt, bulge, 0, 0);
                }
            }

            pl.Closed = true;
            pl.Layer = layerName;
            pl.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorIndex
            );

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(pl, btr, tr);
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

        public static void DrawDbTextOnMidLinePoint(
            Line line,
            double textValue,
            Transaction tr,
            BlockTableRecord btr,
            string textUnit,
            double textHeight = 1.2,
            int colorIndex = 1,
            string layer = "E-HOMERUN"
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

            // Crear texto con la energía
            DBText dbText = new DBText
            {
                Position = displacedMid,
                TextString = $"{textValue:0.00} {textUnit}",
                Height = textHeight,
                Layer = layer,
                ColorIndex = colorIndex,
                Rotation = angle
            };

            // Primero establecer los modos de alineación
            dbText.HorizontalMode = TextHorizontalMode.TextCenter;
            dbText.VerticalMode = TextVerticalMode.TextVerticalMid;

            // Luego el punto de alineación
            dbText.AlignmentPoint = displacedMid;

            // Finalmente, ajustar la alineación
            dbText.AdjustAlignment(btr.Database);

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(dbText, btr, tr);
        }

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

        public static List<string> WrapText(string input, int maxLineLength = 30)
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



