using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;
using System;
using TYPSA.SharedLib.Autocad.ShowInfoBox;
using static TYPSA.SharedLib.Autocad.ProcessPoly.cls_00_ProcessOffsetPolyResult;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ProcessAllOffsetPoly
    {
        public static ProcessOffsetPolyResult ProcessAllOffsetPoly(
            List<Polyline> polysToOffset,
            Transaction tr,
            BlockTableRecord btr,
            string entityTag,
            double offsetDistance
        )
        {
            // Contadores poly desfasadas
            int allOffsetPolyCount = 0;
            int nullOffsetPolyCount = 0;
            int validOffsetPolyCount = 0;

            // Conjunto para almacenar las polilíneas que no han podido ser desfasadas
            HashSet<ObjectId> offsetPolyToIsolate = new HashSet<ObjectId>();

            // Lista para almacenar polilíneas desfasadas válidas para su posterior procesamiento
            List<Polyline> validOffsetPoly = new List<Polyline>();
            List<Polyline> validOffsetPolyAndPoly = new List<Polyline>();

            // Diccionario para almacenar la relación entre la polilínea original y la desfasada
            Dictionary<Handle, Handle> dictPolyToOffsetPoly =
                new Dictionary<Handle, Handle>();

            // StringBuilder para almacenar la información de polylines desfasadas
            StringBuilder infoOffsetPoly = new StringBuilder();

            // Recorremos las poly iniciales válidas
            foreach (Polyline polyToOffset in polysToOffset)
            {
                // Procesamos la offset poly
                cls_00_ProcessOffsetPoly.ProcessOffsetPoly(
                    polyToOffset, offsetDistance, tr, btr, offsetPolyToIsolate, infoOffsetPoly,
                    dictPolyToOffsetPoly, validOffsetPoly, validOffsetPolyAndPoly,
                    ref allOffsetPolyCount, ref nullOffsetPolyCount, ref validOffsetPolyCount,
                    entityTag
                );
            }

            // Agregar el conteo total al inicio del `StringBuilder`
            infoOffsetPoly.Insert(0,
                $"📌 Summary of offset {entityTag} Polylines:{Environment.NewLine}" +
                $"🔹 Total offset {entityTag} Polylines processed: {allOffsetPolyCount}{Environment.NewLine}" +
                $"⚠ Null or invalid offset {entityTag} Polylines: {nullOffsetPolyCount}{Environment.NewLine}" +
                $"✅ Successfully processed offset {entityTag} Polylines: {validOffsetPolyCount}{Environment.NewLine}{Environment.NewLine}"
            );


            // Mostrar info
            if (nullOffsetPolyCount > 0)
            {
                cls_00_ShowInfoBox.ShowStringBuilder(
                    $"⚠ Offset {entityTag} Issues Found:",
                    infoOffsetPoly.ToString()
                );
            }

            return new ProcessOffsetPolyResult
            {
                ValidOffsetPolylines = validOffsetPoly,
                ValidOffsetAndOriginalPolys = validOffsetPolyAndPoly,
                OffsetPolylinesToIsolate = offsetPolyToIsolate,
                DictPolyToOffset = dictPolyToOffsetPoly,
                InfoSummary = infoOffsetPoly,
                OffsetDistance = offsetDistance,
                Total = allOffsetPolyCount,
                NullCount = nullOffsetPolyCount,
                ValidCount = validOffsetPolyCount
            };
        }



    }
}
