using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckBlockRefsInLayoutsCount
    {
        private static List<BlockRefsByLayoutResult> CheckBlockRefsInLayoutsCount(
            Transaction tr,
            BlockTable bt,
            string fileName
        )
        {
            List<BlockRefsByLayoutResult> results = new List<BlockRefsByLayoutResult>();

            // -----------------------------
            // Iterar layouts
            // -----------------------------

            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord layoutBtr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

                // Validamos
                if (layoutBtr == null) continue;
                if (!layoutBtr.IsLayout) continue;

                // -----------------------------
                // Obtener layout
                // -----------------------------

                Layout layout = tr.GetObject(layoutBtr.LayoutId, OpenMode.ForRead) as Layout;
                // Validamos
                if (layout == null) continue;

                // Obviamos Model Space
                if (layout.ModelType) continue;

                // -----------------------------
                // Obtener Block References
                // -----------------------------

                List<BlockRefInfo> blockReferences = new List<BlockRefInfo>();
                // Iteramos
                foreach (ObjectId entId in layoutBtr)
                {
                    try
                    {
                        BlockReference br = tr.GetObject(entId, OpenMode.ForRead) as BlockReference;
                        // Validamos
                        if (br == null) continue;

                        BlockTableRecord brBtr = tr.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        // Validamos
                        if (brBtr == null) continue;

                        // Añadimos
                        blockReferences.Add(new BlockRefInfo
                        {
                            BlockRefName = brBtr.Name,
                            Handle = br.Handle.ToString(),
                            Layer = br.Layer
                        }
                        );
                    }
                    catch
                    {
                        continue;
                    }
                }

                // -----------------------------
                // Almacenar resultado Layout
                // -----------------------------

                results.Add(new BlockRefsByLayoutResult
                {
                    FileName = fileName,
                    LayoutName = layout.LayoutName,
                    HasBlockRefs = blockReferences.Count > 0,
                    BlockRefCount = blockReferences.Count,
                    BlockReferences = blockReferences
                }
                );
            }

            // return
            return results;
        }

        public class BlockRefsByLayoutResult
        {
            [JsonIgnore]
            public string FileName { get; set; }
            public string LayoutName { get; set; }
            public bool HasBlockRefs { get; set; }
            public int BlockRefCount { get; set; }
            public List<BlockRefInfo> BlockReferences { get; set; }
        }

        public class BlockRefInfo
        {
            public string BlockRefName { get; set; }
            [JsonIgnore]
            public string Handle { get; set; }
            [JsonIgnore]
            public string Layer { get; set; }
        }

        public static List<BlockRefsByLayoutResult> AnalyzeBlockRefsInLayoutsCount(
            Transaction tr,
            BlockTable bt,
            string fileName
        )
        {
            return CheckBlockRefsInLayoutsCount(
                tr, bt, fileName
            );
        }
    }
}
