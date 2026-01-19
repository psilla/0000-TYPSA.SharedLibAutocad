using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetLayerNamesFromDocOrXref
    {
        public static List<string> GetLayerNamesFromDocOrXref(
            BlockTable bt,
            Transaction tr,
            bool isXrefDocument,
            string xrefFilePath = null
        )
        {
            // Definimos por defecto
            List<string> layers = null;

            // Xref
            if (isXrefDocument)
            {
                // Obtenemos todas las capas filtradas
                layers = cls_00_GetLayerNamesFromXref.GetLayerNamesFromXref(bt, tr, xrefFilePath);
            }
            // Active Doc
            else
            {
                // Obtenemos todas las capas filtradas
                layers = cls_00_GetLayerNamesFromDocFilt.GetLayerNamesFromDocFilt();
            }
            // return
            return (layers != null && layers.Count > 0) ? layers : null;
        }



    }
}
