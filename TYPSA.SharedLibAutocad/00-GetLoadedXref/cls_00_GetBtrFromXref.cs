using System.Collections.Generic;
using System.IO;
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

        public static bool TryGetXrefInfo(
            BlockTable bt,
            Transaction tr,
            out BlockTableRecord xrefBtr,
            out string xrefFilePath
        )
        {
            // Inicializamos
            xrefBtr = null;
            xrefFilePath = null;

            // -----------------------------
            // Obtener BlockTableRecord
            // -----------------------------

            xrefBtr = cls_00_GetBtrFromXref.GetBtrFromXref(bt, tr);
            // Validamos
            if (xrefBtr == null) return false;
           
            // -----------------------------
            // Obtener ruta del Xref
            // -----------------------------

            xrefFilePath = xrefBtr.PathName;

            // Convertir a ruta absoluta si es relativa
            if (!Path.IsPathRooted(xrefFilePath))
            {
                string currentDrawingDir = Path.GetDirectoryName(
                    HostApplicationServices.Current.FindFile(
                        Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager
                            .MdiActiveDocument.Name,
                        Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager
                            .MdiActiveDocument.Database,
                        FindFileHint.Default
                    )
                );
                // Obtenemos ruta completa del archivo Xref
                xrefFilePath = Path.Combine(currentDrawingDir, xrefFilePath);
            }

            // return
            return true;
        }



    }
}
