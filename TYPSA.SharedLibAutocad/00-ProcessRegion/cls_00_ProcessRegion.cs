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
            Region regionFromPoly = ConvertPolyToRegion(
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

        public static Region ConvertPolyToRegion(
            Polyline poly,
            Transaction tr,
            BlockTableRecord btr,
            Dictionary<Handle, string> failedPoly,
            Dictionary<Handle, Region> diccRegiones
        )
        {
            // try
            try
            {
                // Obtener el Handle de la polilínea
                Handle polyHandle = poly.Handle;

                // Validamos si la polilínea ya tiene una región creada en el diccionario
                if (diccRegiones.ContainsKey(polyHandle))
                {
                    // Retornar la región existente
                    return diccRegiones[polyHandle];
                }
                // Validamos
                if (poly == null || !poly.Closed)
                {
                    if (poly != null && !poly.Closed)
                    {
                        // Cerrar la polilínea
                        poly.Closed = true;
                    }
                    else
                    {
                        // Registrar la polilínea fallida
                        failedPoly[polyHandle] = "Invalid or unclosed polyline.";
                        // Finalizamos
                        return null;
                    }
                }

                // Crear una colección de curvas de AutoCAD
                DBObjectCollection curvas = new DBObjectCollection();

                // Descomponer la polilínea en segmentos
                poly.Explode(curvas);
                // Validamos curvas
                if (curvas.Count == 0)
                {
                    // Registrar la polilínea fallida
                    failedPoly[polyHandle] = "Failed to extract valid curves.";
                    // Finalizamos
                    return null;
                }

                // Crear región a partir de las curvas filtradas
                DBObjectCollection regiones = Region.CreateFromCurves(curvas);
                // Validamos
                if (regiones.Count == 0)
                {
                    // Registrar la polilínea fallida
                    failedPoly[polyHandle] = "Conversion to region failed.";
                    // Finalizamos
                    return null;
                }

                // Obtener la primera región creada
                Region region = regiones[0] as Region;
                // Validamos
                if (region == null)
                {
                    // Registrar la polilínea fallida
                    failedPoly[polyHandle] = "Could not get the created region.";
                    // Finalizamos
                    return null;
                }

                // Ajustar la capa de la región para que sea la misma que la de la polilínea 🔹**
                region.Layer = poly.Layer;

                // Agregar a la BlockTableRecord
                cls_00_DocumentInfo.AddEntityToBlockTableRecord(region, btr, tr);

                // Guardar la región en el diccionario para evitar duplicados
                diccRegiones[polyHandle] = region;

                // return
                return region;
            }

            // catch
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                // Registrar la polilínea fallida
                failedPoly[poly.Handle] = $"Error en AutoCAD: {ex.Message}";

                // Finalizamos
                return null;
            }
        }



    }
}
