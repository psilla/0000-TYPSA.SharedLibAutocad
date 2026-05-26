using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.GetLayersInfo;
using TYPSA.SharedLib.Autocad.Main;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetMTextByLayer
    {
        public static PromptSelectionResult GetMTextByLayer(
            Editor ed,
            List<string> docLayers,
            string entityTag,
            string layerNameByDefault = null
        )
        {
            // Pedimos la capa al usuario
            string layerName = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                docLayers, entityTag, layerNameByDefault
            );

            // En caso de null, finalizamos
            if (layerName == null) return null;

            EntityTypes entityTypes = EntityTypes.GetDefaultEntityTypes();
            // Definir el filtro 
            var filtros = new TypedValue[]
            {
                new TypedValue((int)DxfCode.LayerName, layerName),
                new TypedValue((int)DxfCode.Start, entityTypes.MText)
                //new TypedValue((int)DxfCode.Operator, "<OR"),
                //new TypedValue((int)DxfCode.Start, entityTypes.Text),
                //new TypedValue((int)DxfCode.Start, entityTypes.MText),
                //new TypedValue((int)DxfCode.Operator, "OR>")
            };

            // Definimos la selección
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

        public static PromptSelectionResult GetMTextByLayers(
            Editor ed,
            List<string> docLayers,
            string entityTag,
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

            EntityTypes entityTypes = EntityTypes.GetDefaultEntityTypes();
            // Tipo de entidad
            filterValues.Add(new TypedValue((int)DxfCode.Start, entityTypes.MText));

            // Realizar la selección
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
