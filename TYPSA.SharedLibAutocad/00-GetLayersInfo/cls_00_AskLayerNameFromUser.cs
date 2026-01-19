using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_AskLayerNameFromUser
    {
        public static string AskLayerNameFromUser(
            List<string> layers,
            string objeto,
            string layerNameByDefault = null
        )
        {
            // Ordenamos las capas
            List<string> orderedLayers = layers
                .OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Mostramos formulario con CheckList
            string selectedLayer = InstanciarFormularios.CheckListBoxFormUniqueSelectionSearchOut(
                $"Select the layer that contains the {objeto}:",
                orderedLayers, layerNameByDefault
            );
            // Validamos
            if (string.IsNullOrEmpty(selectedLayer))
            {
                // Mensaje
                MessageBox.Show(
                    "⚠ No layer was selected. Operation cancelled.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                // Finalizamos
                return null;
            }
            // return
            return selectedLayer;
        }

        public static List<string> AskLayerNamesFromUser(
            List<string> layers,
            string objeto,
            List<string> layerNamesByDefault = null
        )
        {
            // Ordenamos las capas
            List<string> orderedLayers = layers
                .OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Mostramos formulario con CheckListBox
            List<string> selectedLayers = InstanciarFormularios.CheckListBoxFormSearchOut(
                $"Select the layers that contain the {objeto}:",
                orderedLayers, layerNamesByDefault
            );
            // Validamos
            if (selectedLayers == null || selectedLayers.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    "⚠ No layer was selected. Operation cancelled.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                // Finalizamos
                return null;
            }
            // return
            return selectedLayers;
        }






    }
}
