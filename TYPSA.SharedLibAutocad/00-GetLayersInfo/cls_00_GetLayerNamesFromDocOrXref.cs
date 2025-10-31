using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetLayerNamesFromDocOrXref
    {
        public static List<string> GetLayerNamesFromDocOrXref(
            BlockTable bt,
            Transaction tr,
            bool boolXrefOrDoc,
            string xrefFilePath = null
        )
        {
            // Definimos por defecto
            List<string> layers = null;

            // Active Doc
            if (!boolXrefOrDoc)
            {
                // Obtenemos todas las capas filtradas
                layers = cls_00_GetLayerNamesFromDocFilt.GetLayerNamesFromDocFilt();
            }
            // Xref
            else
            {
                // Obtenemos todas las capas filtradas
                layers = cls_00_GetLayerNamesFromXref.GetLayerNamesFromXref(bt, tr, xrefFilePath);
            }
            // return
            return (layers != null && layers.Count > 0) ? layers : null;
        }



    }
}
