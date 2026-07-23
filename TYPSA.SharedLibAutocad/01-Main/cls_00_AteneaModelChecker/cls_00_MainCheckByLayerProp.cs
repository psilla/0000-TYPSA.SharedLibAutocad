using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckByLayerProp
    {
        private static List<ByLayerEntityResult> CheckByLayerUsage(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            bool isSpanish
        )
        {
            List<ByLayerEntityResult> results = new List<ByLayerEntityResult>();

            // -----------------------------
            // Textos localizados
            // -----------------------------

            string errorText = isSpanish ? "Error" : "Error";
            string unknownText = isSpanish ? "Desconocido" : "Unknown";

            // -----------------------------
            // Iterar espacios
            // -----------------------------

            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                // Busca en espacio Papel y Modelo, no busca en xRef
                if (!btr.IsLayout) continue;

                // Espacio modelo o papel
                string space = btr.Name;

                // -----------------------------
                // Iterar entidades
                // -----------------------------

                foreach (ObjectId entId in btr)
                {
                    try
                    {
                        // Obtenemos
                        Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                        // Validamos
                        if (ent == null) continue;

                        // Opcional pero recomendable
                        if (ent.IsAProxy) continue;

                        // Obtenemos tipo de entidad
                        string entityType = ent.GetType().Name;

                        bool? isColorByLayer = null;
                        bool? isLinetypeByLayer = null;
                        bool? isLineweightByLayer = null;

                        string colorValue = unknownText;
                        string linetypeValue = unknownText;
                        string lineweightValue = unknownText;

                        // -----------------------------
                        // Color
                        // -----------------------------

                        try
                        {
                            isColorByLayer = ent.Color.IsByLayer;
                            colorValue = ent.Color.IsByLayer
                                ? "ByLayer"
                                : ent.Color.ColorNameForDisplay;
                        }
                        catch
                        {
                            colorValue = errorText;
                        }

                        // -----------------------------
                        // Linetype
                        // -----------------------------

                        try
                        {
                            isLinetypeByLayer = ent.LinetypeId == db.ByLayerLinetype;
                            linetypeValue = ent.Linetype;
                        }
                        catch
                        {
                            linetypeValue = errorText;
                        }

                        // -----------------------------
                        // Lineweight
                        // -----------------------------

                        try
                        {
                            isLineweightByLayer = ent.LineWeight == LineWeight.ByLayer;
                            lineweightValue = ent.LineWeight.ToString();
                        }
                        catch
                        {
                            lineweightValue = errorText;
                        }

                        // -----------------------------
                        // Almacenamos
                        // -----------------------------

                        results.Add(new ByLayerEntityResult
                        {
                            FileName = fileName,
                            Handle = ent.Handle.ToString(),
                            Layer = ent.Layer,
                            EntityType = entityType,
                            Space = space,
                            IsColorByLayer = isColorByLayer,
                            ColorValue = colorValue,
                            IsLinetypeByLayer = isLinetypeByLayer,
                            LinetypeValue = linetypeValue,
                            IsLineweightByLayer = isLineweightByLayer,
                            LineweightValue = lineweightValue
                        });
                    }
                    catch
                    {
                        // Obviamos
                        continue;
                    }
                }
            }

            // return
            return results;
        }

        public class ByLayerEntityResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }

            public string Handle { get; set; }

            public string Layer { get; set; }

            public string EntityType { get; set; }

            public string Space { get; set; }

            // -----------------------------
            // Color
            // -----------------------------

            public bool? IsColorByLayer { get; set; }
            public string ColorValue { get; set; }

            // -----------------------------
            // Linetype
            // -----------------------------

            public bool? IsLinetypeByLayer { get; set; }
            public string LinetypeValue { get; set; }

            // -----------------------------
            // Lineweight
            // -----------------------------

            public bool? IsLineweightByLayer { get; set; }
            public string LineweightValue { get; set; }
        }

        public static List<ByLayerEntityResult> AnalyzeByLayer(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName,
            bool isSpanish
        )
        {
            // return
            return CheckByLayerUsage(tr, db, bt, fileName, isSpanish);
        }

        public static void SetByLayerProperties(
            Transaction tr,
            Database db,
            BlockTable bt
        )
        {
            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr =
                    (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);

                if (!btr.IsLayout) continue;

                foreach (ObjectId entId in btr)
                {
                    Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
                    if (ent == null) continue;

                    if (ent.IsAProxy) continue;

                    // -----------------------------
                    // Color → ByLayer
                    // -----------------------------

                    if (!ent.Color.IsByLayer)
                    {
                        ent.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                            Autodesk.AutoCAD.Colors.ColorMethod.ByLayer,
                            256
                        );
                    }

                    // -----------------------------
                    // Linetype → ByLayer
                    // -----------------------------

                    if (ent.LinetypeId != db.ByLayerLinetype)
                    {
                        ent.LinetypeId = db.ByLayerLinetype;
                    }

                    // -----------------------------
                    // Lineweight → ByLayer
                    // -----------------------------

                    if (ent.LineWeight != LineWeight.ByLayer)
                    {
                        ent.LineWeight = LineWeight.ByLayer;
                    }
                }
            }
        }


    }
}
