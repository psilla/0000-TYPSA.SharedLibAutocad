using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetLoadedXref
{
    public class cls_00_GetDictLoadedXref
    {
        public static Dictionary<string, BlockTableRecord> GetDictLoadedXrefs(
            BlockTable bt,
            Transaction tr
        )
        {
            // Definimos diccionario
            Dictionary<string, BlockTableRecord> xrefs = new Dictionary<string, BlockTableRecord>();

            // Recorremos todos los registros de bloques del dibujo
            foreach (ObjectId btrId in bt)
            {
                // Abrimos cada BlockTableRecord en modo lectura
                BlockTableRecord btr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

                // Solo añadimos los que son referencias externas (Xrefs)
                if (btr != null && btr.IsFromExternalReference)
                {
                    // Almacenamos en el diccionario
                    xrefs[btr.Name] = btr;
                }
            }

            // return
            return xrefs;
        }




    }
}
