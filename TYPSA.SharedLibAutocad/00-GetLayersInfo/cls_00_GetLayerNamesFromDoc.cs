using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetLayerNamesFromDoc
    {
        public class LayerInfo
        {
            public string Name { get; set; }
            public bool IsOn { get; set; }
            public bool IsFrozen { get; set; }
            public bool IsLocked { get; set; }
            public bool IsPlottable { get; set; }
        }

        public static List<string> GetLayerNamesFromDoc(
            Database db
        )
        {
            List<string> layerNames = new List<string>();
            // Abrimos transaccion
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Accedemos a la tabla de capas en modo lectura
                LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                // Recorremos las capas
                foreach (ObjectId layerId in layerTable)
                {
                    // Obtenemos la capa
                    LayerTableRecord layer = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);

                    // Agregamos
                    layerNames.Add(layer.Name);
                }

                // Cerramos transaccion
                tr.Commit();
            }

            // Ordenar alfabeticamente
            return layerNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static List<LayerInfo> GetLayerInfoFromDoc(
            Transaction tr,
            Database db
        )
        {
            List<LayerInfo> layers = new List<LayerInfo>();

            LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

            foreach (ObjectId layerId in lt)
            {
                LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);

                layers.Add(new LayerInfo
                {
                    Name = ltr.Name,
                    IsOn = !ltr.IsOff,
                    IsFrozen = ltr.IsFrozen,
                    IsLocked = ltr.IsLocked,
                    IsPlottable = ltr.IsPlottable
                });
            }

            return layers
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }





    }
}
