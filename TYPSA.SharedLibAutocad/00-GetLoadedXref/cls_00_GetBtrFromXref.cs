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
            Dictionary<string, BlockTableRecord> dictXrefs =
                cls_00_GetDictLoadedXref.GetDictLoadedXrefs(bt, tr);
            // Validamos
            if (dictXrefs == null || dictXrefs.Count == 0) return null;

            // Obtenemos el listado de nombres para el form
            List<string> dictXrefsKeys = dictXrefs.Keys.ToList();

            // Form para elegir el Xref
            string selectedXref = cls_00_InstaForm_CheckedListBox.CheckListBoxFormUniqueSelectionSearchOut(
                "Select the XREF to analyze:", dictXrefsKeys, dictXrefsKeys.First()
            );
            // Validamos
            if (string.IsNullOrWhiteSpace(selectedXref)) return null;

            // Obtenemos la BlockTableRecord desde el dict
            BlockTableRecord xrefBtr = dictXrefs[selectedXref];

            // return
            return xrefBtr;
        }



    }
}
