using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Linq;
using System.Text;
using TYPSA.SharedLib.Autocad.DeleteEntities;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.ProcessRegion
{
    public class cls_00_ProcessRegion
    {
        public static void ProcessRegion(
            Polyline contornoOffset,
            Transaction tr,
            BlockTableRecord btr,
            Dictionary<Handle, string> failedPolylines,
            Dictionary<Handle, Region> diccRegiones,
            HashSet<ObjectId> offsetPolyToRegionToIsolate,
            StringBuilder infoOffsetPolyToRegion,
            Dictionary<Handle, Handle> dictPolyToOffsetPoly,
            Dictionary<Handle, Handle> dictPolyToRegion,
            List<Region> validRegion,
            List<Polyline> validOffsetPoly,
            ref int allRegionCount,
            ref int nullRegionCount,
            ref int validRegionCount,
            string entityTag
        )
        {
            // Contar polilíneas convertidas a región (válidas o nulas)
            allRegionCount++;

            // Convertimos la polilinea desfasada a región
            Region regionFromPoly = cls_00_ConvertPolyToRegion.ConvertPolyToRegion(
                contornoOffset, tr, btr, failedPolylines, diccRegiones
            );
            // Validamos
            if (regionFromPoly == null)
            {
                // Obtener el motivo del fallo desde el diccionario (si existe)
                string motivoFallo = failedPolylines.ContainsKey(contornoOffset.Handle)
                    ? failedPolylines[contornoOffset.Handle]
                    : "Unknown reason.";

                // Obtenemos la capa
                string layer = contornoOffset.Layer;

                // Agregar al resumen de polilíneas fallidas con el ObjectId y el motivo del fallo
                infoOffsetPolyToRegion.AppendLine(
                    $"⚠ Failed to convert {entityTag} Offset Polyline to region → " +
                    $"ObjectId: {contornoOffset.ObjectId}, " +
                    $"Handle: {contornoOffset.Handle}, " +
                    $"Layer: {layer} | " +
                    $"Reason: {motivoFallo}"
                );

                // Contador de fallos en la conversión
                nullRegionCount++;

                // Agregar la polilínea no válida al conjunto de aislamiento
                offsetPolyToRegionToIsolate.Add(contornoOffset.ObjectId);

                // Finalizamos
                return;
            }

            // Buscar la polilínea original en el diccionario usando Handle
            if (dictPolyToOffsetPoly.ContainsValue(contornoOffset.Handle))
            {
                Handle originalHandle =
                    dictPolyToOffsetPoly.FirstOrDefault(kvp => kvp.Value == contornoOffset.Handle).Key;

                // Guardamos la relación solo con los Handles
                dictPolyToRegion[originalHandle] = regionFromPoly.Handle;
            }

            // Asegurar que está en ForWrite antes de modificar
            regionFromPoly.UpgradeOpen();

            // Obtenemos la capa
            string regionLayer = regionFromPoly.Layer;

            // Agregar al resumen de polilíneas procesadas correctamente con el Handle de la región creada
            infoOffsetPolyToRegion.AppendLine(
                $"✔ Region successfully created from {entityTag} Offset Polyline → " +
                $"Region Handle: {regionFromPoly.Handle}, " +
                $"From Offset Polyline Handle: {contornoOffset.Handle}, " +
                $"Layer: {regionLayer}"
            );

            // Contador de conversiones exitosas
            validRegionCount++;

            // Agregar la región creada
            validRegion.Add(regionFromPoly);

            // Borramos la polilínea en caso de ser una desfasada y no original
            if (validOffsetPoly.Contains(contornoOffset))
            {
                // Borrar la polilínea
                cls_00_DeleteEntity.DeleteEntity(contornoOffset);
            }
        }

        



    }
}
