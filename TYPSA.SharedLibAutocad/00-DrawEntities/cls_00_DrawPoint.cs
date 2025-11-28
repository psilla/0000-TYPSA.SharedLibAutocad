using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawPoint
    {
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
