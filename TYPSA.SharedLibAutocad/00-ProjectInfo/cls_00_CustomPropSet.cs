using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;
using System;

namespace TYPSA.SharedLib.Autocad.ProjectCustomProp
{
    public class cls_00_CustomPropSet
    {
        public static void SetCustomProperty(
            Document doc,
            string propertyName,
            string propertyValue
        )
        {
            Database db = cls_00_DocumentInfo.GetDatabaseFromDocument(doc);

            // try
            try
            {
                // Desbloquear la base de datos antes de modificarla
                using (DocumentLock docLock = doc.LockDocument())
                {
                    // Abrir transaccion
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

                            // Verificar si la propiedad ya existe
                            if (customProps.Contains(propertyName))
                            {
                                // Actualizar su valor
                                customProps[propertyName] = propertyValue;
                            }
                            else
                            {
                                // Agregarla
                                customProps.Add(propertyName, propertyValue);
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
                                $"Error setting the property '{propertyName}': {ex.Message}",
                                "Error"
                            );
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Mensaje
                MessageBox.Show(
                    $"Error locking the document: {ex.Message}",
                    "Error"
                );
            }
        }

        public static void SetCustomPropertiesFromDict(
            Document doc,
            Dictionary<string, string> propiedades
        )
        {
            // Iteramos
            foreach (var propiedad in propiedades)
            {
                // Llamar a la función para establecer la propiedad en Civil 3D
                SetCustomProperty(doc, propiedad.Key, propiedad.Value);
            }
        }





    }
}
