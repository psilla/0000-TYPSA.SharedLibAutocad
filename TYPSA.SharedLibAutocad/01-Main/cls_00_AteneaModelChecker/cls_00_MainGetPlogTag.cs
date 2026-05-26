using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainGetPlogTag
    {
        private static List<PlotInfoResult> CheckPlotTagInfo(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            List<string> referenceTexts,
            string blockNameFilter
        )
        {
            List<PlotInfoResult> results = new List<PlotInfoResult>();

            // Iteramos layouts
            foreach (ObjectId btrId in bt)
            {
                // Obtenemos btr
                BlockTableRecord layoutBtr = tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

                // Validamos
                if (!layoutBtr.IsLayout) continue;
                // Excluimos espacio Modelo
                if (layoutBtr.Name == BlockTableRecord.ModelSpace) continue;

                // -----------------------------
                // Obtener Layout
                // -----------------------------

                Layout layout = tr.GetObject(layoutBtr.LayoutId, OpenMode.ForRead) as Layout;
                // Validamos
                if (layout == null) continue;

                // Nombre de la pestaña
                string layoutName = layout.LayoutName;

                // -----------------------------
                // Iteramos entidades layout
                // -----------------------------

                foreach (ObjectId entId in layoutBtr)
                {
                    try
                    {
                        // Obtenemos entidad
                        Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                        // Validamos
                        if (!(ent is BlockReference br)) continue;

                        // Obtenemos btr del bloque
                        BlockTableRecord brBtr = tr.GetObject(br.BlockTableRecord,OpenMode.ForRead) as BlockTableRecord;
                        // Obtenemos nombre
                        string blockName = brBtr.Name;

                        // Filtramos
                        if (string.IsNullOrWhiteSpace(blockName) ||
                            !blockName.Contains(blockNameFilter)) continue;

                        // -----------------------------
                        // Obtener textos internos
                        // -----------------------------

                        List<TextData> texts = new List<TextData>();
                        // Iteramos
                        foreach (ObjectId nestedId in brBtr)
                        {
                            // Obtenemos entidad
                            Entity nestedEnt = tr.GetObject(nestedId, OpenMode.ForRead) as Entity;
                            // Validamos
                            if (nestedEnt == null) continue;

                            // DBText
                            if (nestedEnt is DBText dbText)
                            {
                                texts.Add(new TextData
                                {
                                    Text = dbText.TextString,
                                    Position = dbText.Position
                                });
                            }

                            // MText
                            else if (nestedEnt is MText mText)
                            {
                                texts.Add(new TextData
                                {
                                    Text = mText.Text,
                                    Position = mText.Location
                                });
                            }
                        }

                        // -----------------------------
                        // Buscar referencias
                        // -----------------------------

                        foreach (string refText in referenceTexts)
                        {
                            TextData reference = texts.FirstOrDefault(x =>
                                x.Text != null &&
                                x.Text.Contains(refText)
                            );

                            // -----------------------------
                            // Resultado base
                            // -----------------------------

                            PlotInfoResult result = new PlotInfoResult
                            {
                                FileName = fileName,
                                LayoutName = layoutName,
                                BlockRefName = blockName,
                                Handle = br.Handle.ToString(),
                                ReferenceText = refText,
                                IsFound = false,
                                TextValue = null
                            };

                            // Validamos referencia
                            if (reference != null)
                            {
                                // -----------------------------
                                // Buscar texto por encima
                                // -----------------------------

                                TextData upperText = texts
                                    .Where(x =>
                                        x != reference &&
                                        x.Position.Y > reference.Position.Y &&
                                        !referenceTexts.Any(r =>
                                            x.Text != null &&
                                            x.Text.Contains(r)
                                        )
                                    )
                                    .OrderBy(x => x.Position.DistanceTo(reference.Position))
                                    .FirstOrDefault();

                                // Validamos
                                if (upperText != null)
                                {
                                    result.IsFound = true;
                                    result.TextValue = upperText.Text;
                                }
                            }

                            // Añadimos siempre
                            results.Add(result);
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

        private class TextData
        {
            public string Text { get; set; }

            public Point3d Position { get; set; }
        }

        public class PlotInfoResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }

            public string LayoutName { get; set; }

            public string BlockRefName { get; set; }

            public string Handle { get; set; }

            public string ReferenceText { get; set; }

            public string TextValue { get; set; }
            public bool IsFound { get; set; }
        }

        public static List<PlotInfoResult> AnalyzePlotTagInfo(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            List<string> referenceTexts,
            string blockNameFilter
        )
        {
            return CheckPlotTagInfo(
                tr, db, bt, fileName, referenceTexts, blockNameFilter
            );
        }
    }
}
