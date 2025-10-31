using System.Collections;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;
using System;
using System.Collections.Generic;

namespace TYPSA.SharedLib.Autocad.ProjectCustomProp
{
    public class cls_00_CustomPropDel
    {
        public static void RemoveCustomProperty(
            Document doc,
            string propertyName
        )
        {
            Database db = cls_00_DocumentInfo.GetDatabaseFromDocument(doc);

            // try
            try
            {
                using (DocumentLock docLock = doc.LockDocument())
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // try
                        try
                        {
                            // Obtener la información resumida del dibujo
                            DatabaseSummaryInfoBuilder summaryInfoBuilder =
                                new DatabaseSummaryInfoBuilder(db.SummaryInfo);

                            // Obtener la tabla de propiedades personalizadas
                            IDictionary customProps = summaryInfoBuilder.CustomPropertyTable;

                            // En caso de contener la propiedad
                            if (customProps.Contains(propertyName))
                            {
                                // Eliminamos
                                customProps.Remove(propertyName);
                            }

                            // Aplicar los cambios a la base de datos
                            db.SummaryInfo = summaryInfoBuilder.ToDatabaseSummaryInfo();

                            // Cerrar la transacción
                            tr.Commit();
                        }
                        // catch
                        catch (Exception ex)
                        {
                            // Mensaje
                            MessageBox.Show(
                                $"Error deleting property '{propertyName}': {ex.Message}",
                                "Error"
                            );
                        }
                    }
                }
            }
            // catch
            catch (Exception ex)
            {
                // Mensaje
                MessageBox.Show(
                    $"Error locking document: {ex.Message}", "Error"
                );
            }
        }

        public static void RemoveCustomPropertiesFromDict(
            Document doc,
            List<string> keysToRemove
        )
        {
            // Iteramos
            foreach (var key in keysToRemove)
            {
                RemoveCustomProperty(doc, key);
            }
        }






    }
}
