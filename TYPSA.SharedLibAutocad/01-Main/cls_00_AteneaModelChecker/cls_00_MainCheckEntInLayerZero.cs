using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckEntInLayerZero
    {
        private static LayerZeroUsageResult CheckLayerZeroUsage(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            // ID real de ModelSpace
            ObjectId modelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);

            // Contadores
            int modelCount = 0;
            int paperCount = 0;

            // Iteramos
            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                // Busca en espacio papel y modelo
                if (!btr.IsLayout) continue;

                bool isModelSpace = btrId == modelSpaceId;
                // Iteramos
                foreach (ObjectId entId in btr)
                {
                    // Obtenemos entidad
                    Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                    // Validamos
                    if (ent == null) continue;

                    // Validamos layer Zero
                    if (ent.Layer != "0") continue;

                    // Acumulamos
                    if (isModelSpace)
                        modelCount++;
                    else
                        paperCount++;
                }
            }

            // Total
            int totalCount = modelCount + paperCount;

            // return
            return new LayerZeroUsageResult
            {
                FileName = fileName,
                IsUsed = totalCount > 0,
                EntityCount = totalCount,
                ModelSpaceCount = modelCount,
                PaperSpaceCount = paperCount
            };
        }

        public class LayerZeroUsageResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }
            // Total
            public bool IsUsed { get; set; }
            // Total entidades en capa 0
            public int EntityCount { get; set; }
            // Modelo
            public int ModelSpaceCount { get; set; }
            // Papel
            public int PaperSpaceCount { get; set; }
        }

        public static LayerZeroUsageResult AnalyzeLayerZero(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            // return
            return CheckLayerZeroUsage(
                tr, db, bt, fileName
            );
        }
    }
}
