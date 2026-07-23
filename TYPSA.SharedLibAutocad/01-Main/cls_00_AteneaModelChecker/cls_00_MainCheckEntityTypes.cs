using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckEntityTypes
    {
        private static List<EntityTypeResult> CheckEntityTypes(
            Transaction tr,
            Database db,
            string fileName
        )
        {
            List<EntityTypeResult> results = new List<EntityTypeResult>();
            BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

            // Iteramos
            foreach (ObjectId id in btr)
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                // Validamos
                if (ent == null) continue;

                // Añadimos
                results.Add(new EntityTypeResult
                {
                    FileName = fileName,
                    Category = ent.GetType().Name,
                    Layer = ent.Layer,
                    Handle = ent.Handle.ToString()
                });
            }

            // return
            return results;
        }

        public class EntityTypeResult
        {
            [JsonIgnore]
            public string FileName { get; set; }
            public string Category { get; set; }
            public string Layer { get; set; }
            public string Handle { get; set; }
        }

        public static List<EntityTypeResult> AnalyzeEntityTypes(
            Transaction tr,
            Database db,
            string fileName
        )
        {
            return CheckEntityTypes(
                tr, db, fileName
            );
        }
    }
}
