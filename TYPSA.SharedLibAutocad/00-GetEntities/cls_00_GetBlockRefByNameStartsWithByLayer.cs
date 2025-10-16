using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.GetLayersInfo;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetBlockRefByNameStartsWithByLayer
    {
        public static PromptSelectionResult GetBlockRefByNameStartsWithAndByLayer(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            string startsWith,
            string layerNameByDefault = null
        )
        {
            // Pedimos la capa al usuario
            string layerName = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                docLayers, entityTag, layerNameByDefault
            );

            // En caso de null, finalizamos
            if (layerName == null) return null;

            // Definir el filtro por nombre de bloque y tipo de entidad
            var filtros = new TypedValue[]
            {
                new TypedValue((int)DxfCode.LayerName, layerName),
                new TypedValue((int)DxfCode.Start, "INSERT"),
                //new TypedValue((int)DxfCode.BlockName, startsWith)
            };

            // Realizar la selección
            PromptSelectionResult psr =
                cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(ed, filtros);

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
