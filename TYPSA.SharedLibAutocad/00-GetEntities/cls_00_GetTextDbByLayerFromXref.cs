using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetTextDbByLayerFromXref
    {
        public static bool GetTextDbByLayerFromXref(
            Transaction tr,
            List<string> layersFromXref,
            BlockTableRecord xrefBtr,
            string selectedLayer,
            out HashSet<ObjectId> textIds
        )
        {
            textIds = null;
            // Filtramos los DBText cuya capa coincida
            HashSet<ObjectId> resultIds = xrefBtr.Cast<ObjectId>()
                .Select(id => tr.GetObject(id, OpenMode.ForRead) as Entity)
                .Where(ent => ent is DBText)
                .Where(ent =>
                {
                    string layerName = ent.Layer.Split('|').Last();
                    return layerName.Equals(selectedLayer, StringComparison.OrdinalIgnoreCase);
                })
                .Select(ent => ent.ObjectId)
                .ToHashSet();
            // Validamos
            if (resultIds.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    $"⚠ No DBText found in layer '{selectedLayer}' " +
                    $"inside Xref '{xrefBtr.Name}'.", "No Matches Found"
                );
                // Finalizamos
                return false;
            }

            // Asignamos los resultados
            textIds = resultIds;
            // return
            return true;
        }

    }
}
