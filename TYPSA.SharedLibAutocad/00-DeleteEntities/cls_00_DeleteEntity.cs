using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.DeleteEntities
{
    public class cls_00_DeleteEntity
    {

        public static void DeleteEntity(DBObject obj)
        {
            if (obj == null || obj.IsErased)
                return;

            // Si no está abierto para escritura, lo abrimos
            if (!obj.IsWriteEnabled)
            {
                try
                {
                    obj.UpgradeOpen();
                }
                catch
                {
                    // No se pudo cambiar a modo escritura
                    return;
                }
            }

            // Si está listo, borramos
            if (!obj.IsErased && obj.IsWriteEnabled)
            {
                obj.Erase();
            }
        }







    }
}
