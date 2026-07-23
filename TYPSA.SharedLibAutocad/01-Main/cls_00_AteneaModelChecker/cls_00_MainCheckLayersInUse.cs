using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;
using static TYPSA.SharedLib.Autocad.GetLayersInfo.cls_00_GetLayerNamesFromDoc;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckLayersInUse
    {
        private static List<LayerUsageResult> CheckLayersUsage(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            // ID real de ModelSpace
            ObjectId modelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);

            // Conteo en espacio modelo
            Dictionary<string, int> modelCount = new Dictionary<string, int>();

            // Conteo en espacio papel
            Dictionary<string, int> paperCount = new Dictionary<string, int>();

            // Iteramos
            foreach (ObjectId btrId in bt)
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                // Busca en espacio papel y modelo
                if (!btr.IsLayout) continue;

                bool isModelSpace = btrId == modelSpaceId;
                // Iteramos entidades
                foreach (ObjectId entId in btr)
                {
                    // Obtenemos entidad
                    Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                    // Validamos
                    if (ent == null) continue;

                    // Obtenemos capa
                    string layer = ent.Layer;

                    // Seleccionamos diccionario en funcion de ubicacion
                    Dictionary<string, int> targetDict = isModelSpace
                        ? modelCount
                        : paperCount;

                    // Inicializamos
                    if (!targetDict.ContainsKey(layer))
                        targetDict[layer] = 0;
                    // Acumulamos
                    targetDict[layer]++;
                }
            }

            // Obtenemos capas
            List<LayerInfo> allLayers = GetLayerInfoFromDoc(tr, db);

            // Resultados
            List<LayerUsageResult> results = new List<LayerUsageResult>();
            // Iteramos capas
            foreach (LayerInfo layer in allLayers)
            {
                // Conteo modelo
                int modelEntities = modelCount.ContainsKey(layer.Name)
                    ? modelCount[layer.Name]
                    : 0;

                // Conteo papel
                int paperEntities = paperCount.ContainsKey(layer.Name)
                    ? paperCount[layer.Name]
                    : 0;

                // Total
                int totalEntities = modelEntities + paperEntities;

                // Añadimos
                results.Add(new LayerUsageResult
                {
                    FileName = fileName,
                    LayerName = layer.Name,
                    IsUsed = totalEntities > 0,
                    EntityCount = totalEntities,
                    ModelSpaceCount = modelEntities,
                    PaperSpaceCount = paperEntities,
                    IsOn = layer.IsOn,
                    IsFrozen = layer.IsFrozen,
                    IsLocked = layer.IsLocked,
                    IsPlottable = layer.IsPlottable,
                    Color = layer.Color,
                    Linetype = layer.Linetype,
                    LineWeight = layer.LineWeight,
                    Transparency = layer.Transparency,
                });
            }

            // return
            return results;
        }

        public class LayerUsageResult
        {
            [JsonIgnore]
            public string FileName { get; set; }
            public string LayerName { get; set; }
            public bool IsUsed { get; set; }
            // Total entidades
            public int EntityCount { get; set; }
            // Entidades en modelo
            public int ModelSpaceCount { get; set; }
            // Entidades en papel
            public int PaperSpaceCount { get; set; }
            public bool IsOn { get; set; }
            public bool IsFrozen { get; set; }
            public bool IsLocked { get; set; }
            public bool IsPlottable { get; set; }
            public string Color { get; set; }
            public string Linetype { get; set; }
            public string LineWeight { get; set; }
            public string Transparency { get; set; }
        }

        public static List<LayerUsageResult> AnalyzeLayers(
            Transaction tr,
            Database db,
            BlockTable bt,
            string fileName
        )
        {
            // return
            return CheckLayersUsage(
                tr, db, bt, fileName
            );
        }
    }
}