using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.GetLayersInfo;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetPolylinesOrBlockRefByLayer
    {
        public static PromptSelectionResult GetPolylinesOrBlockRefByLayer(
            Editor ed,
            List<string> docLayers,
            string entityTag,
            bool boolDcBlocksBool,
            string layerNameByDefault = null
        )
        {
            // Pedimos la capa al usuario
            string layerName =
                cls_00_AskLayerNameFromUser.AskLayerNameFromUser(docLayers, entityTag, layerNameByDefault);

            // En caso de null, finalizamos
            if (layerName == null) return null;

            // Determinar el tipo de entidad a filtrar
            string filtro = boolDcBlocksBool ? "INSERT" : "LWPOLYLINE";

            // Definimos la selección
            PromptSelectionResult psr = cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                ed,
                new TypedValue((int)DxfCode.LayerName, layerName),
                new TypedValue((int)DxfCode.Start, filtro)
            );

            // Verificación
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                // Mensaje
                MessageBox.Show(
                    $"No {entityTag} found.", "Warning"
                );
                // Finalizamos
                return null;
            }

            // return
            return psr;
        }


    }
}
