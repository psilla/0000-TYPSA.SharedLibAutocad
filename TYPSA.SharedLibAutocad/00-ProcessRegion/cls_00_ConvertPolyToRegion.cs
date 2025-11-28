using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.ProcessRegion
{
    public class cls_00_ConvertPolyToRegion
    {
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
