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
            Dictionary<string, string> customProperties = new Dictionary<string, string>();
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

                    // Cerramos transaccion
                    tr.Commit();
                }
                // catch
                catch (System.Exception ex)
                {
                    // Mensaje
                    MessageBox.Show(
                        $"Error retrieving project custom properties:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    // Finalizamos
                    return customProperties;
                }
            }
            // Validamos
            if (customProperties == null || customProperties.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    "No project custom properties were found in the document.",
                    "Project Custom Properties",
                    MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
            // return
            return customProperties;
        }





    }
}
