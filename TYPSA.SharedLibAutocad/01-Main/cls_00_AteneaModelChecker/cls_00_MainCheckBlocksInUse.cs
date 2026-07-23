using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckBlocksInUse
    {
        private static List<BlockUsageResult> CheckBlocksUsage(
            Transaction tr,
            Database db,
            string fileName
        )
        {
            List<BlockUsageResult> results = new List<BlockUsageResult>();
            BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            Dictionary<string, int> blockRefs = new Dictionary<string, int>();

            // Contar referencias
            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                if (btr == null) continue;

                foreach (ObjectId entId in btr)
                {
                    BlockReference br = tr.GetObject(entId, OpenMode.ForRead) as BlockReference;
                    if (br == null) continue;

                    string name = (
                        tr.GetObject(br.BlockTableRecord, OpenMode.ForRead)
                        as BlockTableRecord
                    ).Name;

                    if (!blockRefs.ContainsKey(name))
                        blockRefs[name] = 0;

                    blockRefs[name]++;
                }
            }

            // Revisar bloques
            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;
                if (btr == null || btr.IsLayout || btr.IsAnonymous) continue;

                int count = blockRefs.ContainsKey(btr.Name) ? blockRefs[btr.Name] : 0;

                results.Add(new BlockUsageResult
                {
                    FileName = fileName,
                    BlockName = btr.Name,
                    IsUsed = count > 0,
                    ReferenceCount = count
                });
            }

            // return
            return results;
        }

        public class BlockUsageResult
        {
            [JsonIgnore]
            public string FileName { get; set; }
            public string BlockName { get; set; }
            public bool IsUsed { get; set; }
            public int ReferenceCount { get; set; }
        }

        public static List<BlockUsageResult> AnalyzeBlocks(
            Transaction tr,
            Database db,
            string fileName
        )
        {
            return CheckBlocksUsage(
                tr, db, fileName
            );
        }
    }
}
