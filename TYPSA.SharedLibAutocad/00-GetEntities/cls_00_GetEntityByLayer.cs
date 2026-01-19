using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetLayersInfo;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetEntityByLayer
    {
        public static PromptSelectionResult GetEntityByLayer(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            string entityType,
            string layerNameByDefault = null
        )
        {
            // Pedimos la capa al usuario
            string layerName = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                docLayers, entityTag, layerNameByDefault
            );
            // Validamos
            if (layerName == null) return null;

            // Definir el filtro por nombre de bloque y tipo de entidad
            var filtros = new TypedValue[]
            {
                new TypedValue((int)DxfCode.LayerName, layerName),
                new TypedValue((int)DxfCode.Start, entityType),
            };

            // Realizar la selección
            PromptSelectionResult psr =
                cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(ed, filtros);
            // Validamos
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

        public static PromptSelectionResult GetEntityByLayers(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            string entityType,
            List<string> defaultLayers = null
        )
        {
            // Pedimos las capas al usuario 
            List<string> selectedLayers =
                cls_00_AskLayerNameFromUser.AskLayerNamesFromUser(
                    docLayers, entityTag, defaultLayers
                );
            // Validamos
            if (selectedLayers == null || selectedLayers.Count == 0) return null;

            // Construimos el filtro OR para las capas
            List<TypedValue> filterValues = new List<TypedValue>();

            // Inicio OR
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            // Iteramos
            foreach (string layer in selectedLayers)
            {
                filterValues.Add(new TypedValue((int)DxfCode.LayerName, layer));
            }
            // Fin OR
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            // Tipo de entidad
            filterValues.Add(new TypedValue((int)DxfCode.Start, entityType));

            // Realizar la selección
            PromptSelectionResult psr =
                cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                    ed, filterValues.ToArray()
                );
            // Validamos 
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
