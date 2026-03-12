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
        public static PromptSelectionResult GetAllEntitiesByType(
            Editor ed,
            string entityTag,
            string entityType
        )
        {
            // Definir el filtro solo por tipo de entidad
            var filterValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, entityType),
            };

            // Seleccionamos
            PromptSelectionResult psr = cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                ed, filterValues
            );

            // Validamos
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                MessageBox.Show($"No {entityTag} found.", "Warning");
                return null;
            }
            // return
            return psr;
        }

        public static PromptSelectionResult GetTextAndMTextByLayer(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            out string selectedLayer,
            string layerNameByDefault = null
        )
        {
            selectedLayer = null;
            // Pedimos la capa al usuario
            string layerName = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                docLayers, entityTag, layerNameByDefault
            );
            // Validamos
            if (layerName == null) return null;

            // Asignamos
            selectedLayer = layerName;

            // Filtro OR para MTEXT y TEXT
            var filterValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.LayerName, layerName),

                new TypedValue((int)DxfCode.Operator, "<OR"),
                    new TypedValue((int)DxfCode.Start, "MTEXT"),
                    new TypedValue((int)DxfCode.Start, "TEXT"),
                new TypedValue((int)DxfCode.Operator, "OR>")
            };

            // Seleccionamos
            PromptSelectionResult psr = cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                ed, filterValues
            );
            // Validamos
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                MessageBox.Show($"No {entityTag} found.", "Warning");
                return null;
            }

            return psr;
        }

        public static PromptSelectionResult GetTextAndMTextByLayers(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            out List<string> selectedLayers,
            List<string> defaultLayers = null
        )
        {
            selectedLayers = null;
            // Pedimos las capas al usuario
            List<string> layerNames = cls_00_AskLayerNameFromUser.AskLayerNamesFromUser(
                docLayers, entityTag, defaultLayers
            );
            // Validamos
            if (layerNames == null) return null;

            // Asignamos
            selectedLayers = layerNames;

            // Construimos filtro
            List<TypedValue> filterValues = new List<TypedValue>();

            // OR capas
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            foreach (string layer in layerNames)
            {
                filterValues.Add(new TypedValue((int)DxfCode.LayerName, layer));
            }
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            // OR tipos (TEXT + MTEXT)
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            filterValues.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            filterValues.Add(new TypedValue((int)DxfCode.Start, "MTEXT"));
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            // Seleccionamos
            PromptSelectionResult psr = cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                ed, filterValues.ToArray()
            );
            // Validamos
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                // Mensaje
                MessageBox.Show($"No {entityTag} found.", "Warning");
                // Finalizamos
                return null;
            }
            // return
            return psr;
        }

        public static PromptSelectionResult GetEntityByLayer(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            string entityType,
            out string selectedLayer,
            string layerNameByDefault = null
        )
        {
            selectedLayer = null;
            // Pedimos la capa al usuario
            string layerName = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                docLayers, entityTag, layerNameByDefault
            );
            // Validamos
            if (layerName == null) return null;

            // Asignamos
            selectedLayer = layerName;

            // Definir el filtro por nombre de bloque y tipo de entidad
            var filterValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.LayerName, layerName),
                new TypedValue((int)DxfCode.Start, entityType),
            };

            // Seleccionamos
            PromptSelectionResult psr = cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                ed, filterValues
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

        public static PromptSelectionResult GetEntityByLayers(
            List<string> docLayers,
            Editor ed,
            string entityTag,
            string entityType,
            out List<string> selectedLayers,
            List<string> defaultLayers = null
        )
        {
            selectedLayers = null;
            // Pedimos las capas al usuario 
            List<string> layerNames = cls_00_AskLayerNameFromUser.AskLayerNamesFromUser(
                docLayers, entityTag, defaultLayers
            );
            // Validamos
            if (layerNames == null) return null;

            // Aplicamos
            selectedLayers = layerNames;

            // Construimos el filtro OR para las capas
            List<TypedValue> filterValues = new List<TypedValue>();

            // Construimos filtro OR
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            // Iteramos
            foreach (string layer in layerNames)
            {
                filterValues.Add(new TypedValue((int)DxfCode.LayerName, layer));
            }
            filterValues.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            // Añadimos Tipo de entidad
            filterValues.Add(new TypedValue((int)DxfCode.Start, entityType));

            // Seleccionamos
            PromptSelectionResult psr = cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
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
