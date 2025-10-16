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
            // Mostramos formulario para elegir la capa
            string layerName = InstanciarFormularios.DropDownFormListOut(
                $"Select the layer that contains the {objeto}:",
                layers.OrderBy(l => l, StringComparer.OrdinalIgnoreCase).ToList(),
                "Selection form to choose a project layer",
                layerNameByDefault
            );

            // Validar cancelación
            if (string.IsNullOrEmpty(layerName))
            {
                // Mensaje
                MessageBox.Show("⚠ No layer was selected. Operation cancelled.", "Warning");
                // Finalizamos
                return null;
            }

            // return
            return layerName;
        }





    }
}
