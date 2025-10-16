using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_FilterBlockRefByNameFromPsr
    {
        public static PromptSelectionResult FilterBlockRefsByNameFromPsr(
            PromptSelectionResult psrOriginal,
            Editor ed
        )
        {
            List<BlockReference> allBlockRef = new List<BlockReference>();
            // Iteramos
            foreach (SelectedObject selObj in psrOriginal.Value)
            {
                // Validamos
                if (selObj == null) continue;
                // Obtenemos el objeto
                DBObject obj = selObj.ObjectId.GetObject(OpenMode.ForRead);
                // Validamos
                if (obj is BlockReference br)
                {
                    // Almacenamos
                    allBlockRef.Add(br);
                }
            }

            // Obtener nombres únicos
            List<string> blockRefNames = allBlockRef
                .Select(br => br.Name).Distinct().OrderBy(n => n).ToList();

            // Form
            List<string> blockRefNamesSelected = InstanciarFormularios.
                CheckListBoxFormOut(
                "Select block names to filter:\n" +
                $"Use Ctrl + A / Ctrl + D to Select / Deselect all.",
                blockRefNames
            );
            // Validamos
            if (blockRefNamesSelected == null || blockRefNamesSelected.Count == 0) return null;

            // Filtramos por los nombres seleccionados
            List<BlockReference> filteredBlocks = allBlockRef
                .Where(br => blockRefNamesSelected.Contains(br.Name)).ToList();

            // try
            try
            {
                // Convertimos los bloques filtrados en ObjectIds
                ObjectId[] selectedIds = filteredBlocks.Select(b => b.ObjectId).ToArray();

                // Inyectamos los ObjectId al editor
                ed.SetImpliedSelection(selectedIds);

                // Creamos y devolvemos el nuevo PromptSelectionResult
                return ed.SelectImplied();
            }
            // catch
            catch (System.Exception ex)
            {
                // Mostramos el error
                MessageBox.Show(
                    $"❌ An error occurred while overriding the PromptSelectionResult:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                // Finalizamos
                return null;
            }
        }




    }
}
