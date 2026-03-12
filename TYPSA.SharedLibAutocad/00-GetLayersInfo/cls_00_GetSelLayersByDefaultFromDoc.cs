using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetSelLayersByDefaultFromDoc
    {
        public static HashSet<string> GetSelLayersByDefaultFromDoc(
            HashSet<string> defaultLayers,
            string messagePrompt
        )
        {
            // Form para elegir las capas que contienen los metodos de instalacion
            List<string> layersByUser =
                InstanciarFormularios.CheckListBoxFormSearchOut(
                    messagePrompt + "\n\n" +
                    "By default, the following layers will be automatically selected if they exist in the drawing:\n" +
                    string.Join(", ", defaultLayers),
                    cls_00_GetLayerNamesFromDocFilt.GetLayerNamesFromDocFilt(),
                    defaultLayers.ToList()
                );
            // Validamos
            if (layersByUser == null || layersByUser.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    "You must select at least one layer. " +
                    "The operation will be canceled",
                    "Warning"
                );
                // Finalizamos
                return null;
            }
            // Convertir a HashSet y return
            return new HashSet<string>(layersByUser);
        }



    }
}
