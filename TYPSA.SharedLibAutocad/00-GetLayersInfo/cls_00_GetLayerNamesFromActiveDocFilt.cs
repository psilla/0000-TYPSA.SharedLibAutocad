using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetLayerNamesFromActiveDocFilt
    {
        public static List<string> GetVisiblePrintableUnlockedLayerNames()
        {
            List<string> layerNames = new List<string>();

            // Obtenemos info
            Document doc = cls_00_DocumentInfo.GetActiveDocument();
            Database db = cls_00_DocumentInfo.GetDatabaseFromDocument(doc);

            // Abrimos transaccion
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Accedemos a la tabla de capas en modo lectura
                LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                // Recorremos las capas
                foreach (ObjectId layerId in layerTable)
                {
                    // Obtenemos la capa
                    LayerTableRecord layer = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);

                    // Filtramos las capas apagadas, bloqueadas o que no se imprimen
                    if (!layer.IsOff && !layer.IsLocked && layer.IsPlottable)
                    {
                        // Agregamos
                        layerNames.Add(layer.Name);
                    }
                }

                // Cerramos transaccion
                tr.Commit();
            }

            // Ordenar alfabeticamente
            return layerNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
        }




    }
}
