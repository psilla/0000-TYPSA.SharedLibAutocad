using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckEntityTypesCount
    {
        private static List<EntityTypeResult> CheckEntityTypesCount(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            // ID real de ModelSpace
            ObjectId modelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);

            // Conteo en espacio modelo
            Dictionary<string, int> modelCount =
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Conteo en espacio papel
            Dictionary<string, int> paperCount =
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // -------------------------------
            // Iterar espacios
            // -------------------------------

            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr =
                    tr.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

                // Validamos
                if (btr == null) continue;
                if (!btr.IsLayout) continue;

                bool isModelSpace = btrId == modelSpaceId;

                // -------------------------------
                // Iterar entidades
                // -------------------------------

                foreach (ObjectId entId in btr)
                {
                    Entity ent =
                        tr.GetObject(entId, OpenMode.ForRead) as Entity;
                    // Validamos
                    if (ent == null) continue;

                    // Obtenemos categoria
                    string category = ent.GetType().Name;

                    // Seleccionamos diccionario
                    Dictionary<string, int> targetDict =
                        isModelSpace
                            ? modelCount
                            : paperCount;

                    // Inicializamos
                    if (!targetDict.ContainsKey(category))
                        targetDict[category] = 0;

                    // Acumulamos
                    targetDict[category]++;
                }
            }

            // -------------------------------
            // Obtener categorias encontradas
            // -------------------------------

            List<string> categories = modelCount.Keys
                .Union(paperCount.Keys, StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            // -------------------------------
            // Crear resultados
            // -------------------------------

            List<EntityTypeResult> results =
                new List<EntityTypeResult>();

            // Iteramos
            foreach (string category in categories)
            {
                int modelEntities = modelCount.ContainsKey(category)
                    ? modelCount[category]
                    : 0;

                int paperEntities = paperCount.ContainsKey(category)
                    ? paperCount[category]
                    : 0;

                int totalEntities = modelEntities + paperEntities;

                // Añadimos
                results.Add(new EntityTypeResult
                {
                    FileName = fileName,
                    Category = category,
                    EntityCount = totalEntities,
                    ModelSpaceCount = modelEntities,
                    PaperSpaceCount = paperEntities
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

            public int EntityCount { get; set; }

            public int ModelSpaceCount { get; set; }

            public int PaperSpaceCount { get; set; }
        }

        public static List<EntityTypeResult> AnalyzeEntityTypesCount(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            // return
            return CheckEntityTypesCount(
                tr, db, bt, fileName
            );
        }
    }
}
