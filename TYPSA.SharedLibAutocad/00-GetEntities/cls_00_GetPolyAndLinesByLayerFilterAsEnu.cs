using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetPolyAndLinesByLayerFilterAsEnu
    {
        public static IEnumerable<Entity> GetPolyAndLinesByLayerFilterAsEnu(
            Database db,
            Transaction tr,
            BlockTableRecord btr,
            HashSet<string> layersFilter,
            out int lineCount,
            out int polylineCount,
            out HashSet<string> capasUsadas
        )
        {
            // Inicializar contadores y capas
            lineCount = 0;
            polylineCount = 0;
            capasUsadas = new HashSet<string>();

            // Obtener lineas y poly para los HomeRuns
            DBObjectCollection allLinesAndPoly =
                cls_00_GetPolyAndLinesByLayerFilter.GetPolyAndLinesByLayerFilter(
                    db, tr, btr, layersFilter,
                    out lineCount, out polylineCount, out capasUsadas
                );
            // Validamos
            if (allLinesAndPoly.Count == 0)
            {
                string capasTexto = capasUsadas.Count > 0
                    ? string.Join(", ", capasUsadas)
                    : "None";
                // Mensaje
                MessageBox.Show(
                    "⚠️ No lines or polylines found in the drawing.\n\n" +
                    $"Used layers: {capasTexto}\n\n" +
                    "Null return.",
                    "Warning"
                );
                // Finalizamos
                return null;
            }

            // Creamos un enumerable de entidades a partir de la lista anterior
            IEnumerable<Entity> entitiesFromLinesAndPoly = allLinesAndPoly.Cast<Entity>();
            // Validamos
            if (!entitiesFromLinesAndPoly.Any())
            {
                // Mensaje
                MessageBox.Show(
                    "⚠️ No entities created from the previous lines/polylines. Null return.",
                    "Empty Region"
                );
                // Finalizamos
                return null;
            }
            // return
            return entitiesFromLinesAndPoly;
        }



    }
}
