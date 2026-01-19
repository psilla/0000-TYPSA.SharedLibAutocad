using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;
using TYPSA.SharedLib.Autocad.GetLayersInfo;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetBlockRefByLayerDocOrXref
    {
        public static bool GetBlockRefByLayerDocOrXref(
            bool isXrefDocument,
            Transaction tr,
            BlockTableRecord xrefBtr,
            Editor ed,
            List<string> layersDocOrXref,
            string entityTag,
            string layerNameByDefault,
            string entityType,
            out HashSet<ObjectId> entIds
        )
        {
            entIds = null;
            // Xref
            if (isXrefDocument)
            {
                // Pedimos al usuario seleccionar la capa
                string selectedLayer = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                    layersDocOrXref, entityTag, layerNameByDefault
                );
                // Validamos
                if (selectedLayer == null) return false;

                // Obtenemos los ids
                bool entityBool = cls_00_GetBlockRefByLayerFromXref.GetBlockRefIdsByLayerFromXref(
                    xrefBtr, tr, selectedLayer, out entIds
                );
                // return
                return entityBool;
            }
            // Doc Active
            else
            {
                // Seleccionamos por tipo de objeto y capa
                PromptSelectionResult psrEntity = cls_00_GetEntityByLayer.GetEntityByLayer(
                    layersDocOrXref, ed, entityTag, entityType, layerNameByDefault
                );
                // Validamos
                if (psrEntity == null) return false;

                // Obtenemos los ids
                entIds = new HashSet<ObjectId>(psrEntity.Value.GetObjectIds());
                // return
                return true;
            }
        }
    }
}
