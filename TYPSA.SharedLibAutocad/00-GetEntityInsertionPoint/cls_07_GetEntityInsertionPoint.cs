using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.UserForms;

namespace TYPSA.SharedLib.Autocad.EntitiesInsertionPoint
{
    public class cls_07_GetEntityInsertionPoint
    {
        public static Point3d GetEntityInsertionPoint(Entity ent)
        {
            // Validamos
            if (ent == null) return Point3d.Origin;

            // Casos
            switch (ent)
            {
                case BlockReference blockRef:
                    return blockRef.Position;

                case DBText dbText:
                    return dbText.Position;

                case MText mText:
                    return mText.Location;

                default:
                    // Mensaje
                    new AutoCloseMessageForm(
                        $"⚠ Unsupported entity type: " +
                        $"{ent.GetType().Name}", 1000
                    ).ShowDialog();
                    // Finalizamos
                    return Point3d.Origin;
            }
        }








    }
}
