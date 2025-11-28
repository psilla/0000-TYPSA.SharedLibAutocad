using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawPoly
    {
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


    }
}
