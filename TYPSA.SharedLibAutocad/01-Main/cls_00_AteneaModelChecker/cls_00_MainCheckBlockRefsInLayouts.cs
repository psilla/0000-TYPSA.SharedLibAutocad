using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckBlockRefsInLayouts
    {
        private static List<BlockRefLayoutResult> CheckBlockRefsInLayouts(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            List<BlockRefLayoutResult> results = new List<BlockRefLayoutResult>();

            // -----------------------------
            // Iterar layouts
            // -----------------------------

            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord layoutBtr =
                    tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

                // Validamos
                if (layoutBtr == null) continue;
                if (!layoutBtr.IsLayout) continue;
                if (layoutBtr.Name == BlockTableRecord.ModelSpace) continue;

                // -----------------------------
                // Obtener layout
                // -----------------------------

                Layout layout =
                    tr.GetObject(layoutBtr.LayoutId, OpenMode.ForRead) as Layout;

                if (layout == null) continue;

                string layoutName = layout.LayoutName;

                // -----------------------------
                // Iterar entidades
                // -----------------------------

                foreach (ObjectId entId in layoutBtr)
                {
                    try
                    {
                        BlockReference br =
                            tr.GetObject(entId, OpenMode.ForRead) as BlockReference;

                        if (br == null) continue;

                        BlockTableRecord brBtr =
                            tr.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                        if (brBtr == null) continue;

                        results.Add(new BlockRefLayoutResult
                        {
                            FileName = fileName,
                            LayoutName = layoutName,
                            BlockRefName = brBtr.Name,
                            Handle = br.Handle.ToString(),
                            Layer = br.Layer
                        });
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return results;
        }

        public class BlockRefLayoutResult
        {
            [JsonIgnore]
            public string FileName { get; set; }
            public string LayoutName { get; set; }
            public string BlockRefName { get; set; }
            public string Handle { get; set; }
            public string Layer { get; set; }
        }

        public static List<BlockRefLayoutResult> AnalyzeBlockRefsInLayouts(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            string blockNameFilter = null
        )
        {
            return CheckBlockRefsInLayouts(
                tr, db, bt, fileName
            );
        }
    }
}
