using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;
using System;
using TYPSA.SharedLib.Autocad.ShowInfoBox;
using static TYPSA.SharedLib.Autocad.ProcessRegion.cls_00_ProcessRegionResult;

namespace TYPSA.SharedLib.Autocad.ProcessRegion
{
    public class cls_00_ProcessAllRegion
    {
        public static ProcessRegionResult ProcessAllRegion(
            List<Polyline> validOffsetPolyAndPoly,
            List<Polyline> validOffsetPoly,
            Dictionary<Handle, Handle> dictPolyToOffsetPoly,
            Transaction tr,
            BlockTableRecord btr,
            string entityTag
        )
        {
            // Contadores
            int allRegionCount = 0;
            int nullRegionCount = 0;
            int validRegionCount = 0;

            // Conjunto para almacenar las polilíneas que no se han podido convertir en region
            HashSet<ObjectId> offsetPolyToRegionToIsolate = new HashSet<ObjectId>();

            // StringBuilder para almacenar la información de polylines no convertidas en región
            StringBuilder infoOffsetPolyToRegion = new StringBuilder();

            // Lista para almacenar regiones válidas
            List<Region> validRegion = new List<Region>();

            // Diccionario de polilíneas no convertidas a region
            Dictionary<Handle, string> failedPolylines = new Dictionary<Handle, string>();
            Dictionary<Handle, Region> diccRegiones = new Dictionary<Handle, Region>();

            // Diccionario para almacenar la relación entre la polilínea original y la región
            Dictionary<Handle, Handle> dictPolyToRegion = new Dictionary<Handle, Handle>();

            // Recorrer las poly desfasadas u originales que no fueron desfasadas
            foreach (Polyline contornoOffset in validOffsetPolyAndPoly)
            {
                // Procesamos Region
                cls_00_ProcessRegion.ProcessRegion(
                    contornoOffset, tr, btr, failedPolylines, diccRegiones, offsetPolyToRegionToIsolate,
                    infoOffsetPolyToRegion, dictPolyToOffsetPoly, dictPolyToRegion, validRegion, validOffsetPoly,
                    ref allRegionCount, ref nullRegionCount, ref validRegionCount, entityTag
                );
            }

            // Agregar el conteo total al inicio del `StringBuilder` de infoPolyOffsetToRegion
            infoOffsetPolyToRegion.Insert(0,
                $"📌 Summary of {entityTag} Offset Polylines converted to region:{Environment.NewLine}" +
                $"🔹 Total {entityTag} Offset Polylines attempted to convert: {allRegionCount}{Environment.NewLine}" +
                $"⚠ Failed {entityTag} Offset Polylines conversions: {nullRegionCount}{Environment.NewLine}" +
                $"✅ Successfully converted {entityTag} Offset Polylines to region: {validRegionCount}{Environment.NewLine}{Environment.NewLine}"
            );

            // Mostrar info
            if (nullRegionCount > 0)
            {
                cls_00_ShowInfoBox.ShowStringBuilder(
                    $"⚠ {entityTag} Region Issues Found:",
                    infoOffsetPolyToRegion.ToString()
                );
            }

            // return
            return new ProcessRegionResult
            {
                ValidRegions = validRegion,
                FailedRegionPolylines = offsetPolyToRegionToIsolate,
                FailedPolylineMessages = failedPolylines,
                HandleToRegion = diccRegiones,
                PolyToRegionMap = dictPolyToRegion,
                InfoSummary = infoOffsetPolyToRegion,
                Total = allRegionCount,
                NullCount = nullRegionCount,
                ValidCount = validRegionCount
            };
        }


    }
}
