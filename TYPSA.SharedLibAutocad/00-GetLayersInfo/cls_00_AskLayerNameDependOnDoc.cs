using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_AskLayerNameDependOnDoc
    {
        public static string AskLayerNameDependingOnDocument(
            BlockTable bt,
            Transaction tr,
            bool boolXrefOrDoc,
            string objeto,
            string layerNameByDefault = null,
            string xrefFilePath = null
        )
        {
            // Obtener lista de capas segun el documento
            List<string> layers = cls_00_GetLayerNamesDependOnDoc.GetLayerNamesFromDocOrXref(
                bt, tr, boolXrefOrDoc, xrefFilePath
            );
            // Validamos
            if (layers == null) return null;

            // Si la capa por defecto no existe en la lista, la anulamos
            if (!string.IsNullOrEmpty(layerNameByDefault) && !layers.Contains(layerNameByDefault))
            {
                layerNameByDefault = null;
            }

            // Obtenemos la capa
            string layerByUser = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                layers, objeto, layerNameByDefault
            );
            // Validamos
            if (layerByUser == null) return null;

            // return
            return layerByUser;
        }









    }
}
