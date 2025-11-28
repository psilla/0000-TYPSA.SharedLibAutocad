using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawLine
    {
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


    }
}
