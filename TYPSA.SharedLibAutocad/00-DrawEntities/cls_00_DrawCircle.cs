using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawCircle
    {
        public static void DrawCircle(
            Transaction tr,
            BlockTableRecord btr,
            Point3d center,
            string layerName,
            double radius = 3.0,
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


    }
}
