using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_CreateLayerIfNotExists
    {
        public static void CreateLayerIfNotExists(
            string layerName,
            Database db
        )
        {
            // Abrimos transaccion
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Accedemos a la tabla de capas en modo lectura
                LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                // Validamos si ya existe
                if (layerTable.Has(layerName))
                {
                    // Cerramos transacción
                    tr.Commit();
                    // return
                    return;
                }

                // Cambiamos a modo escritura para añadir nueva capa
                layerTable.UpgradeOpen();

                // Creamos nueva capa
                LayerTableRecord newLayer = new LayerTableRecord
                {
                    Name = layerName
                };

                // Añadimos la capa a la tabla
                layerTable.Add(newLayer);
                tr.AddNewlyCreatedDBObject(newLayer, true);

                // Cerramos la transacción
                tr.Commit();

                // Mensaje
                MessageBox.Show(
                    $"✔ Layer '{layerName}' was created successfully.",
                    "Layer Created"
                );
            }
        }

        public static void CreateLayersIfNotExist(
            IEnumerable<string> layerNames,
            Database db
        )
        {
            // Abrimos transaccion
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Accedemos a la tabla de capas en modo lectura
                LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                bool upgraded = false;
                // Iteramos capas
                foreach (string layerName in layerNames)
                {
                    // Validamos
                    if (string.IsNullOrWhiteSpace(layerName)) continue;

                    // Validamos si ya existe
                    if (layerTable.Has(layerName)) continue;

                    // Abrimos la tabla en escritura solo una vez
                    if (!upgraded)
                    {
                        layerTable.UpgradeOpen();
                        upgraded = true;
                    }

                    // Creamos nueva capa
                    LayerTableRecord newLayer = new LayerTableRecord
                    {
                        Name = layerName
                    };

                    // Añadimos la capa a la tabla
                    layerTable.Add(newLayer);
                    tr.AddNewlyCreatedDBObject(newLayer, true);
                }

                // Cerramos la transacción
                tr.Commit();
            }
        }





    }
}
