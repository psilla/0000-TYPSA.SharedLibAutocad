using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.ZoomToEntity
{
    public class cls_00_ZoomToEntity
    {
        public static void ZoomToEntity(Editor ed, Entity entity)
        {
            // try
            try
            {
                // Obtenemos la geometría
                Extents3d ext = entity.GeometricExtents;

                // Definir el nuevo encuadre de la vista
                Point3d min = ext.MinPoint;
                Point3d max = ext.MaxPoint;
                // Margen para mejor visualización
                double margin = 1.2;

                ViewTableRecord view = new ViewTableRecord();
                view.CenterPoint =
                    new Point2d((min.X + max.X) / 2, (min.Y + max.Y) / 2);
                view.Height = (max.Y - min.Y) * margin;
                view.Width = (max.X - min.X) * margin;

                ed.SetCurrentView(view);
            }
            // catch
            catch (Exception ex)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"⚠ Error zooming to entity: {ex.Message}", 1000
                ).ShowDialog();
            }
        }



    }
}
