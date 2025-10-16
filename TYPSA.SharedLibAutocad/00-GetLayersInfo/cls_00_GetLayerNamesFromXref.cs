using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetLayerNamesFromXref
    {
        public static List<string> GetLayerNamesFromXrefInDocument(
            BlockTable bt, Transaction tr, string xrefFilePath
        )
        {
            // Obtenemos solo el nombre del archivo (sin ruta) para compararlo con los Xrefs cargados
            string xrefName =
                cls_00_DocumentInfo.GetFileNameFromPath(xrefFilePath);

            // Buscamos el BlockTableRecord del Xref ya cargado en el dibujo actual y coincidente con el fileName
            BlockTableRecord xrefBtr =
                cls_00_DocumentInfo.GetLoadedXrefBlockTableRecordByPath(bt, tr, xrefName);
            // Validamos
            if (xrefBtr == null)
            {
                // Mensaje
                MessageBox.Show($"❌ No loaded Xref matches the path:\n{xrefFilePath}", "Xref Not Found");
                // Finalizamos
                return null;
            }

            // Obtenemos la database 
            Database xrefDb =
                cls_00_DocumentInfo.GetDatabaseFromBlockTableRecord(xrefBtr);

            // Obtenemos la layerTable
            LayerTable lt =
                cls_00_DocumentInfo.GetLayerTableForRead(tr, xrefDb);

            // Obtenemos todas las capas visibles, desbloqueadas y que se imprimen del Xref
            List<string> cleanLayers =
                cls_00_GetVisibPrintUnlockLayerNamesFromXref.GetVisibleAndPlottableLayerNamesFromXref(lt, tr);

            // return
            return cleanLayers.
                OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        }




    }
}
