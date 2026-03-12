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
            out HashSet<ObjectId> entIds,
            out string entityLayer
        )
        {
            entIds = null;
            entityLayer = null;
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

                // Asignamos
                entityLayer = selectedLayer;

                // return
                return entityBool;
            }
            // Doc Active
            else
            {
                // Seleccionamos por tipo de objeto y capa
                PromptSelectionResult psrEntity = cls_00_GetEntityByLayer.GetEntityByLayer(
                    layersDocOrXref, ed, entityTag, entityType, out entityLayer, layerNameByDefault
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
