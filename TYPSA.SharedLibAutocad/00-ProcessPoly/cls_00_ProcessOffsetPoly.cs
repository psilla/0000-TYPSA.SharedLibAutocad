using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Linq;
using System.Text;
using TYPSA.SharedLib.Autocad.DeleteEntities;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ProcessOffsetPoly
    {
        public static void ProcessOffsetPoly(
            Polyline polyToOffset,
            double offsetDistance,
            Transaction tr,
            BlockTableRecord btr,
            HashSet<ObjectId> offsetPolyToIsolate,
            StringBuilder infoOffsetPoly,
            Dictionary<Handle, Handle> dictPolyToOffsetPoly,
            List<Polyline> validOffsetPoly,
            List<Polyline> validOffsetPolyAndPoly,
            ref int allOffsetPolyCount,
            ref int nullOffsetPolyCount,
            ref int validOffsetPolyCount,
            string entityTag
        )
        {
            // Contar todas las polilíneas desfasadas (válidas o no)
            allOffsetPolyCount++;

            // Desfasamos la polilínea para asegurarnos de que el bloque entra
            Polyline polyOffset = OffsetPolyDirection(
                polyToOffset, offsetDistance, tr, btr
            );
            // Validamos
            if (polyOffset == null || !polyOffset.Closed)
            {
                // Almacenamos
                offsetPolyToIsolate.Add(polyToOffset.ObjectId);

                // Contador de polilíneas nulas o inválidas
                nullOffsetPolyCount++;

                // Especificar motivo
                string motivo = polyOffset == null
                    ? "offset result was null (likely due to geometry failure)"
                    : "offset polyline is not closed";

                // Agregar información detallada
                infoOffsetPoly.AppendLine(
                    $"⚠ Discarded offset {entityTag} Polyline - " +
                    $"Source ObjectId: {polyToOffset.ObjectId} → " +
                    $"Reason: {motivo}"
                );
                // Finalizamos
                return;
            }

            // Guardamos la relación entre Handles
            dictPolyToOffsetPoly[polyToOffset.Handle] = polyOffset.Handle;

            // Registrar como válida
            int vertices = polyOffset.NumberOfVertices;
            double length = polyOffset.Length;
            string layer = polyOffset.Layer;
            string units = cls_00_DocumentInfo.GetDrawingUnitsName();

            // Mostramos
            infoOffsetPoly.AppendLine(
                $"✔ Offset {entityTag} Polyline processed successfully → " +
                $"Original Poly Handle: {polyToOffset.Handle}, " +
                $"Offset Poly Handle: {polyOffset.Handle} | " +
                $"Vertices: {vertices} | " +
                $"Length: {length:F2} {units} | " +
                $"Layer: {layer}"
            );

            // Contador de polilíneas desfasadas válidas
            validOffsetPolyCount++;

            // Almacenamos la poly desfasada válida
            validOffsetPoly.Add(polyOffset);
            validOffsetPolyAndPoly.Add(polyOffset);
        }

        //public static Polyline OffsetPolyDirection(
        //    Polyline originalPolyline,
        //    double offsetDistance,
        //    Transaction tr,
        //    BlockTableRecord btr
        //)
        //{
        //    // Obtener la longitud de la polilínea original
        //    double originalLength = originalPolyline.Length;

        //    // Hacemos desfase inicial
        //    Polyline offsetPolyline = CreateOffsetPoly(
        //        originalPolyline, offsetDistance, tr, btr
        //    );
        //    // Validamos
        //    if (offsetPolyline == null) return null;

        //    // Si el desfase generó una polilínea más corta, intentamos en el otro sentido
        //    if (offsetPolyline.Length < originalLength)
        //    {
        //        // Eliminar la polilínea desfasada incorrecta
        //        cls_00_DeleteEntity.DeleteEntity(offsetPolyline);
        //        offsetPolyline.Dispose();

        //        // Intentar con el desfase en sentido contrario
        //        offsetPolyline = CreateOffsetPoly(
        //            originalPolyline, -offsetDistance, tr, btr
        //        );
        //        // Validamos
        //        if (offsetPolyline == null) return null;
        //    }

        //    // return
        //    return offsetPolyline;
        //}

        public static Polyline OffsetPolyDirection(
            Polyline originalPolyline,
            double offsetDistance,
            Transaction tr,
            BlockTableRecord btr
        )
        {
            // Longitud de la polilínea original
            double originalLength = originalPolyline.Length;

            // Generar desfase en ambas direcciones
            Polyline offsetPolyPos = CreateOffsetPoly(originalPolyline, offsetDistance, tr, btr);
            Polyline offsetPolyNeg = CreateOffsetPoly(originalPolyline, -offsetDistance, tr, btr);

            // Si ambos fallan, devolver la original
            if (offsetPolyPos == null && offsetPolyNeg == null)
                return originalPolyline;

            // Si solo uno existe, validamos si es más largo que la original
            if (offsetPolyPos != null && (offsetPolyNeg == null || offsetPolyPos.Length > offsetPolyNeg.Length))
            {
                if (offsetPolyPos.Length > originalLength)
                {
                    // Borrar el otro si existe
                    if (offsetPolyNeg != null)
                    {
                        cls_00_DeleteEntity.DeleteEntity(offsetPolyNeg);
                        offsetPolyNeg.Dispose();
                    }
                    return offsetPolyPos;
                }
                else
                {
                    // No es más largo → devolver original
                    cls_00_DeleteEntity.DeleteEntity(offsetPolyPos);
                    offsetPolyPos.Dispose();
                    if (offsetPolyNeg != null)
                    {
                        cls_00_DeleteEntity.DeleteEntity(offsetPolyNeg);
                        offsetPolyNeg.Dispose();
                    }
                    return originalPolyline;
                }
            }
            else if (offsetPolyNeg != null)
            {
                if (offsetPolyNeg.Length > originalLength)
                {
                    // Borrar el otro si existe
                    if (offsetPolyPos != null)
                    {
                        cls_00_DeleteEntity.DeleteEntity(offsetPolyPos);
                        offsetPolyPos.Dispose();
                    }
                    return offsetPolyNeg;
                }
                else
                {
                    // No es más largo → devolver original
                    cls_00_DeleteEntity.DeleteEntity(offsetPolyNeg);
                    offsetPolyNeg.Dispose();
                    if (offsetPolyPos != null)
                    {
                        cls_00_DeleteEntity.DeleteEntity(offsetPolyPos);
                        offsetPolyPos.Dispose();
                    }
                    return originalPolyline;
                }
            }

            // Fallback por seguridad
            return originalPolyline;
        }



        public static Polyline CreateOffsetPoly(
            Polyline poly,
            double offsetDistance,
            Transaction tr,
            BlockTableRecord btr
        )
        {
            // Obtener las curvas desfasadas
            DBObjectCollection offsetCurves = poly.GetOffsetCurves(offsetDistance);

            // Lista para amlacenar poly
            List<Polyline> offsetPolylines = new List<Polyline>();

            // Revisar si el offset generó múltiples resultados y filtrar polilíneas
            foreach (DBObject obj in offsetCurves)
            {
                // En caso de ser poly
                if (obj is Polyline offsetPoly)
                {
                    // Almacenamos
                    offsetPolylines.Add(offsetPoly);
                }
            }
            // Validamos
            if (offsetPolylines.Count == 0) return null;

            // Tomamos la polilínea con el mayor perímetro (evita fragmentos pequeños)
            Polyline bestOffsetPolyline = offsetPolylines.OrderByDescending(p => p.Length).First();

            // Asignar la misma capa de la línea a la poly
            bestOffsetPolyline.Layer = poly.Layer;

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(bestOffsetPolyline, btr, tr);

            // return
            return bestOffsetPolyline;
        }



    }
}
