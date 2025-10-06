using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.SelectEntities
{
    public class cls_00_PromptSelectionWithFilter
    {
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
                    AllowDuplicates = false // Evita duplicados
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
