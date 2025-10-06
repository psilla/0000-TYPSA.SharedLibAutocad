using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.SelectEntities
{
    public class cls_00_GetAllSelectionByFilter
    {
        public static PromptSelectionResult GetAllSelectionByFilter(
            Editor ed,
            params TypedValue[] filtros
        )
        {
            // Si no hay filtros, seleccionar todo
            SelectionFilter sf = (filtros != null && filtros.Length > 0) ? new SelectionFilter(filtros) : null;

            // Ejecutar la selección con los filtros
            PromptSelectionResult psr = ed.SelectAll(sf);

            // return
            return psr;
        }


    }
}
