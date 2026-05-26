using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckBlockAttr
    {
        private static List<BlockAttributesResult> CheckBlockAttributesInLayouts(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            string blockNameFilter 
        )
        {
            List<BlockAttributesResult> results = new List<BlockAttributesResult>();

            // -----------------------------
            // Iterar espacios
            // -----------------------------

            foreach (ObjectId btrId in bt)
            {
                // Obtenemos btr
                BlockTableRecord layoutBtr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);

                // Validamos
                if (!layoutBtr.IsLayout) continue;
                // Validamos solo Papel
                if (layoutBtr.Name == BlockTableRecord.ModelSpace) continue;

                // -----------------------------
                // Obtener Layout
                // -----------------------------

                Layout layout = tr.GetObject(layoutBtr.LayoutId, OpenMode.ForRead) as Layout;
                // Validamos
                if (layout == null) continue;

                // Nombre de la pestaña
                string layoutName = layout.LayoutName;

                // Iteramos
                foreach (ObjectId entId in layoutBtr)
                {
                    try
                    {
                        // Obtenemos entidad
                        Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                        if (ent == null) continue;

                        // Validamos BlockRef
                        if (ent is BlockReference br)
                        {
                            // Obtenemos
                            BlockTableRecord brBtr =
                                (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);

                            string blockName = brBtr.Name;
                            // Filtramos por nombre
                            if (string.IsNullOrWhiteSpace(blockName) ||
                                !blockName.Contains(blockNameFilter)) continue;

                            // Recorremos atributos
                            foreach (ObjectId attId in br.AttributeCollection)
                            {
                                // Obtenemos
                                AttributeReference att =
                                    tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                // Validamos
                                if (att == null) continue;

                                // Almacenamos
                                results.Add(new BlockAttributesResult
                                {
                                    FileName = fileName,
                                    LayoutName = layoutName,
                                    BlockRefName = blockName,
                                    Handle = br.Handle.ToString(),
                                    TagName = att.Tag,
                                    TagValue = att.TextString
                                });
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return results;
        }

        public class BlockAttributesResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }

            public string LayoutName { get; set; }

            public string BlockRefName { get; set; }

            public string Handle { get; set; }

            public string TagName { get; set; }

            public string TagValue { get; set; }
        }

        public static List<BlockAttributesResult> AnalyzeBlockAttributes(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            string blockNameFilter
        )
        {
            return CheckBlockAttributesInLayouts(tr, db, bt, fileName, blockNameFilter);
        }
    }
}
