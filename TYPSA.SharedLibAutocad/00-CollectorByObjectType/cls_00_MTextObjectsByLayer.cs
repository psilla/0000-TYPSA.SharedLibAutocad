using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.DbObjectsByType;
using TYPSA.SharedLib.Autocad.GetDocument;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.ObjectsByTypeByLayer
{
    public class AutocadSettings
    {
        // Separadores validos
        public char[] ValidSeparators { get; set; } = { '.', '-', '_', ',', ';' };
        // Tokens que NO deben crear un nuevo campo
        public List<string> NonSplittableTokens { get; set; } = new List<string>{ "+/", "+/-", "-/+", "+", "-" };

        public static AutocadSettings GetDefaultSettings()
        {
            return new AutocadSettings();
        }
    }
    public class cls_00_MTextObjectsByLayer
    {
        public static string GetAlphabeticFieldKey(
            string fieldValue
        )
        {
            // Validamos
            if (string.IsNullOrWhiteSpace(fieldValue)) return null;

            // Extrae solo letras 
            string key = new string(fieldValue
                .Where(char.IsLetter)
                .ToArray());
            // return
            return key;
        }

        public static bool AllLabelsHaveSameFieldCount(
            Transaction tr,
            IEnumerable<ObjectId> labelIds,
            AutocadSettings autoSettings,
            out int fieldCount,
            out List<string> referenceFields
        )
        {
            fieldCount = -1;
            referenceFields = null;
            // Iteramos
            foreach (ObjectId id in labelIds)
            {
                // Obtenemos objeto
                DBObject dbObj = tr.GetObject(id, OpenMode.ForRead);

                string labelValue = null;
                // Validamos tipo
                if (dbObj is MText mText)
                    labelValue = mText.Contents;
                else if (dbObj is DBText dbText)
                    labelValue = dbText.TextString;
                else
                    continue;

                // Extraemos campos
                List<string> fieldValues = SplitLabelValueByCondAndToken(autoSettings, labelValue);
                // Validamos
                if (fieldValues == null || fieldValues.Count == 0) continue;
                // Validamos
                if (fieldCount < 0)
                {
                    fieldCount = fieldValues.Count;
                    referenceFields = new List<string>(fieldValues);
                }
                else if (fieldValues.Count != fieldCount)
                {
                    // Mensaje
                    MessageBox.Show(
                        "All labels must have the same number of fields.\n" +
                        $"Expected: {fieldCount}, Found: {fieldValues.Count}",
                        "Invalid Label Format",
                        MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    // Finalizamos
                    return false;
                }
            }
            // Validamos
            if (fieldCount < 0 || referenceFields == null)
            {
                // Mensaje
                MessageBox.Show(
                    "No valid labels were found to process.", "Invalid Labels",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                // Finalizamos
                return false;
            }
            // return
            return true;
        }

        public static List<string> GetMTextValues(
            List<DBObject> mtextObjects
        )
        {
            List<string> values = new List<string>();
            // Iteramos
            foreach (var obj in mtextObjects)
            {
                // Validamos
                if (obj is MText mtxt)
                {
                    // Obtener el contenido del MText
                    string textValue = mtxt.Contents;

                    // Validamos y limpiamos
                    if (!string.IsNullOrWhiteSpace(textValue))
                    {
                        // Quitar saltos de línea y formateos de AutoCAD
                        textValue = textValue.Replace("\r", " ")
                                             .Replace("\n", " ")
                                             .Replace("\\P", " ")
                                             .Replace("\\~", " ");
                        // Añadimos
                        values.Add(textValue.Trim());
                    }
                }
            }
            // return
            return values;
        }

        public static List<List<string>> SplitLabelValuesByCond(
            AutocadSettings autoSettings,
            List<string> etiquetas
        )
        {
            // Separadores posibles
            char[] validSeparators = autoSettings.ValidSeparators;

            List<List<string>> result = new List<List<string>>();
            // Iteramos sobre cada etiqueta
            foreach (string et in etiquetas)
            {
                if (string.IsNullOrWhiteSpace(et)) continue;

                // Separar por cualquiera de los separadores válidos
                List<string> parts = et.Split(validSeparators, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(p => p.Trim())
                                       .ToList();

                // Solo agregamos si hay partes válidas
                if (parts.Count > 0)
                    result.Add(parts);
            }

            return result;
        }

        public static List<string> SplitLabelValueByCond(
            AutocadSettings autoSettings,
            string labelValue
        )
        {
            // Separadores posibles
            char[] validSeparators = autoSettings.ValidSeparators;

            // Validamos
            if (string.IsNullOrWhiteSpace(labelValue))
                return new List<string>();

            // Separar por cualquiera de los separadores válidos
            List<string> parts = labelValue
                .Split(validSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            // return
            return parts;
        }

        public static List<string> SplitLabelValueByCondAndToken(
            AutocadSettings autoSettings,
            string labelValue
        )
        {
            char[] validSeparators = autoSettings.ValidSeparators;
            // Split inicial
            List<string> rawParts = labelValue
                .Split(validSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            List<string> result = new List<string>();

            foreach (string part in rawParts)
            {
                if (IsNonSplittableToken(autoSettings, part) && result.Count > 0)
                {
                    // Se adjunta al campo anterior
                    result[result.Count - 1] += " " + part;
                }
                else
                {
                    result.Add(part);
                }
            }

            return result;
        }

        private static bool IsNonSplittableToken(
            AutocadSettings settings,
            string value
        )
        {
            // return
            return settings.NonSplittableTokens
                .Any(t => string.Equals(t, value, StringComparison.Ordinal));
        }

        public static char GetLabelSeparator(
            AutocadSettings autoSettings,
            string labelValue
        )
        {
            // Separadores posibles
            char[] validSeparators = autoSettings.ValidSeparators;
            // Iteramos
            foreach (char c in labelValue)
            {
                // Validamos
                if (validSeparators.Contains(c)) return c;
            }
            // Por defecto
            return ' ';
        }

        

        

        











    }
}
