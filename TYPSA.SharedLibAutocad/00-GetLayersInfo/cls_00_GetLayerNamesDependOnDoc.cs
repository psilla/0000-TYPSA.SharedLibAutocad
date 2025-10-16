using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetLayerNamesDependOnDoc
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
                layers = cls_00_GetLayerNamesFromActiveDocFilt.GetVisiblePrintableUnlockedLayerNames();
            }
            // Xref
            else
            {
                // Obtenemos todas las capas filtradas
                layers = cls_00_GetLayerNamesFromXref.GetLayerNamesFromXrefInDocument(bt, tr, xrefFilePath);
            }
            // return
            return (layers != null && layers.Count > 0) ? layers : null;
        }



    }
}
