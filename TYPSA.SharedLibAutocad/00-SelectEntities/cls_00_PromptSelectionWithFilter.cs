using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.SelectEntities
{
    public class cls_00_PromptSelectionWithFilter
    {
        public static SelectionSet GetSelectionSetByFilter(
            Editor ed,
            string selectionMessage,
            params TypedValue[] filtros
        )
        {
            // -----------------------------
            // Selección personalizada
            // -----------------------------

            PromptSelectionResult psr = PromptSelectionWithFilter(
                ed, selectionMessage, filtros
            );
            // Validamos
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                // Mensaje
                MessageBox.Show(
                    "No entities selected.", "Selection Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                // Finalizamos
                return null;
            }

            // return
            return psr.Value;
        }
        public static PromptSelectionResult PromptSelectionWithFilter(
            Editor ed,
            string mensaje,
            params TypedValue[] filtros
        )
        {
            // try
            try
            {
                PromptSelectionOptions pso = new PromptSelectionOptions
                {
                    MessageForAdding = mensaje,
                    AllowDuplicates = false,
                    SingleOnly = false,
                    SinglePickInSpace = false
                };

                SelectionFilter filter = null;

                // Solo aplicamos el filtro si hay valores
                if (filtros != null && filtros.Length > 0)
                {
                    filter = new SelectionFilter(filtros);
                }

                // return
                return ed.GetSelection(pso, filter);
            }
            // catch
            catch (System.Exception ex)
            {
                // Mensaje
                ed.WriteMessage($"\n❌ ERROR: {ex.Message}");
                // Finalizamos
                return null;
            }
        }

       








    }
}
