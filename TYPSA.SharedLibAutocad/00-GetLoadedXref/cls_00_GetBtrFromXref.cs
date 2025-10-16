using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.GetLoadedXref
{
    public class cls_00_GetBtrFromXref
    {
        public static BlockTableRecord GetBtrFromXref(
        BlockTable bt,
        Transaction tr
    )
        {
            // Obtenemos el diccionario de Xref
            Dictionary<string, BlockTableRecord> DictXrefs =
                cls_00_GetDictLoadedXref.GetDictLoadedXrefs(bt, tr);
            // Validamos
            if (DictXrefs == null || DictXrefs.Count == 0) return null;

            // Obtenemos el listado de nombres para el form
            List<string> DictXrefsKeys = DictXrefs.Keys.ToList();

            // Form para elegir el Xref
            string selectedXref =
                InstanciarFormularios.CheckListBoxFormUniqueSelectionOut(
                    "Select the XREF to analyze:", DictXrefsKeys
                );
            // Validamos
            if (string.IsNullOrWhiteSpace(selectedXref)) return null;

            // Obtenemos la BlockTableRecord desde el dict
            BlockTableRecord xrefBtr = DictXrefs[selectedXref];
            // return
            return xrefBtr;
        }



    }
}
