using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.ProjectCustomProp
{
    public static class cls_00_CustomPropGet
    {
        public static Dictionary<string, string> GetCustomProperties(
            this Database db
        )
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Enumerado de las customProperties
            IDictionaryEnumerator dictEnum = db.SummaryInfo.CustomProperties;
            while (dictEnum.MoveNext())
            {
                DictionaryEntry entry = dictEnum.Entry;

                // Añadir clave:valor
                result.Add((string)entry.Key, (string)entry.Value);
            }
            // return
            return result;
        }

        public static Dictionary<string, string> GetProjectCustomProperties(
            Document doc
        )
        {
            Editor ed = doc.Editor;
            Dictionary<string, string> customProperties = new Dictionary<string, string>();

            // Definimos variable
            string msg = "Project Custom Properties";

            // Abrimos transaccion
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                // try
                try
                {
                    // Abre la base de datos del documento para lectura
                    Database db = doc.Database;

                    // Obtener las propiedades personalizadas usando la extensión
                    customProperties = GetCustomProperties(db);

                    // Construye el mensaje con las propiedades personalizadas
                    StringBuilder messageProperties = new StringBuilder();
                    messageProperties.AppendLine($"{msg}:");
                    // Validamos
                    if (customProperties.Count > 0)
                    {
                        // Iteramos
                        foreach (var prop in customProperties)
                        {
                            // Añadimos
                            messageProperties.AppendLine(
                                $"{prop.Key}: {prop.Value}"
                            );
                        }
                    }
                    else
                    {
                        // Añadimos
                        messageProperties.AppendLine(
                            $"No {msg} were found in the document."
                        );
                    }

                    //// Mensaje
                    //MessageBox.Show(
                    //    messageProperties.ToString(),
                    //    "Project Information"
                    //);

                    // Cerramos transaccion
                    tr.Commit();
                }
                // catch
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"\nError: {ex.Message}");
                    MessageBox.Show(
                        $"Error: {ex.Message}",
                        "Error"
                    );
                }
            }
            // return
            return customProperties;
        }





    }
}
