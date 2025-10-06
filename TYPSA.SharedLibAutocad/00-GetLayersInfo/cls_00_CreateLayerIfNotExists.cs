using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_CreateLayerIfNotExists
    {
        public static void CreateLayerIfNotExists(
            string layerName
        )
        {
            // Obtenemos info
            Document doc = cls_00_DocumentInfo.GetActiveDocument();
            Database db = cls_00_DocumentInfo.GetDatabaseFromDocument(doc);

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




    }
}
