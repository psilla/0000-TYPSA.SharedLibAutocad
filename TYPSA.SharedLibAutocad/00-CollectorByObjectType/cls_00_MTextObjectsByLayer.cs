using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Linq;
using System;
using TYPSA.SharedLib.Autocad.DbObjectsByType;
using TYPSA.SharedLib.Autocad.GetDocument;
//using TYPSA.SharedLib.Autocad.UserForms;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.ObjectsByTypeByLayer
{
    public class cls_00_MTextObjectsByLayer
    {
        public static List<DBObject> get_MTextObjectsByLayer_FromDicc(
            Document doc,
            bool filterByLayer = false
        )
        {
            // Obtenemos el nombre del documento sin extensión
            string docName = cls_00_DocumentInfo.GetActiveDocumentName(doc);

            // Obtenemos el diccionario original
            Dictionary<string, List<DBObject>> diccEntities =
                cls_00_DbObjectsByType.dicc_DbObjects_ByType(doc);
            // Validamos
            if (diccEntities == null || diccEntities.Count == 0 ||
                !diccEntities.ContainsKey("MText"))
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No MText entities were found in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }

            // Nos quedamos solo con los MText
            List<DBObject> mtextObjects = diccEntities["MText"];
            // Validamos
            if (mtextObjects == null || mtextObjects.Count == 0)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No MText entities were found in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }

            // Si no filtramos por capa, devolvemos directamente
            if (!filterByLayer) return mtextObjects;

            // Obtener capas unicas de esos objetos
            HashSet<string> allLayers = new HashSet<string>();
            // Iteramos
            foreach (var obj in mtextObjects.OfType<Entity>())
            {
                // Almacenamos
                allLayers.Add(obj.Layer);
            }

            // Mostrar CheckList para capas
            List<string> selectedLayers = InstanciarFormularios.CheckListBoxFormOut(
                $"Select the Layers to analyze in '{docName}'.\n" +
                $"Use Ctrl + A / Ctrl + D to Select / Deselect all.",
                allLayers.OrderBy(x => x).ToList()
            );
            // Validamos
            if (selectedLayers == null || selectedLayers.Count == 0)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No Layers were selected in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }

            // Filtrar objetos por las capas seleccionadas
            List<DBObject> objetos = mtextObjects
                .Where(obj => obj is Entity ent && selectedLayers.Contains(ent.Layer))
                .ToList();
            // Validamos
            if (objetos == null || objetos.Count == 0)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No MText entities were found after filtering by layer in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }
            // return
            return objetos;
        }


        public static List<string> GetMTextValues(List<DBObject> mtextObjects)
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

        public static List<List<string>> SplitLabelValuesByCond(List<string> etiquetas)
        {
            // Separadores válidos
            char[] validSeparators = { '.', '-', '_', ',', ';' };

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










    }
}
