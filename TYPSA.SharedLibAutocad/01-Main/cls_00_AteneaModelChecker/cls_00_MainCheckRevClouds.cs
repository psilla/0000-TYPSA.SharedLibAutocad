using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckRevClouds
    {
        private static List<RevisionCloudResult> CheckRevisionClouds(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            List<RevisionCloudResult> results = new List<RevisionCloudResult>();

            // -----------------------------
            // Iterar espacios
            // -----------------------------

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

                // Iteramos entidades del layout
                foreach (ObjectId entId in layoutBtr)
                {
                    try
                    {
                        Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                        // Validamos
                        if (ent == null) continue;

                        // Validamos
                        if (ent is Polyline pl)
                        {
                            ResultBuffer rb = pl.GetXDataForApplication("RevcloudProps");
                            // Validamos
                            if (rb != null)
                            {
                                // Almacenamos
                                results.Add(new RevisionCloudResult
                                {
                                    FileName = fileName,
                                    LayoutName = layoutName,
                                    Handle = pl.Handle.ToString(),
                                    Layer = pl.Layer
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

        public class RevisionCloudResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }

            public string LayoutName { get; set; }

            public string Handle { get; set; }

            public string Layer { get; set; }
        }

        public static List<RevisionCloudResult> AnalyzeRevisionClouds(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            return CheckRevisionClouds(tr, db, bt, fileName);
        }
    }
}
