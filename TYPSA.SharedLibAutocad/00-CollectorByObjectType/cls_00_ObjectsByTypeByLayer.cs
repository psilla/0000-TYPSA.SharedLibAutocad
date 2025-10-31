using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Linq;
using TYPSA.SharedLib.Autocad.GetDocument;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.DbObjectsByType
{
    public class cls_00_ObjectsByTypeByLayer
    {
        public static List<DBObject> get_DBObjectsByTypeByLayer_FromDicc(
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
            if (diccEntities == null || diccEntities.Count == 0)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No Entities were found in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }

            // Selección por tipo de objeto
            List<string> availableTypes = diccEntities.Keys.ToList();
            List<string> selectedTypes = InstanciarFormularios.CheckListBoxFormOut(
                $"Select the Objects to analyze in '{docName}'.\n" +
                $"Use Ctrl + A / Ctrl + D to Select / Deselect all.",
                availableTypes.OrderBy(x => x).ToList()
            );
            // Validamos
            if (selectedTypes == null || selectedTypes.Count == 0)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No Entities were selected in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }

            // Filtrar diccionario por tipo
            Dictionary<string, List<DBObject>> diccEntitiesFiltered =
                diccEntities
                    .Where(kvp => selectedTypes.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Extraer objetos combinados por tipo
            List<DBObject> filteredObjects =
                get_ListValues_FromDicc(diccEntitiesFiltered);

            // Si no se requiere filtrar por capa, devolver directamente
            if (!filterByLayer)
            {
                // Validamos
                if (filteredObjects == null || filteredObjects.Count == 0)
                {
                    // Mensaje
                    new AutoCloseMessageForm(
                        $"No objects found after filtering by type in '{docName}'.\n\n" +
                        $"This file will not be processed."
                    ).ShowDialog();
                    // Finalizamos
                    return null;
                }
                else
                {
                    // return
                    return filteredObjects;
                }
            }

            // Obtener capas únicas de esos objetos
            HashSet<string> allLayers = new HashSet<string>();
            foreach (var obj in filteredObjects.OfType<Entity>())
            {
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

            // Filtrar objetos por capas seleccionadas
            List<DBObject> objetos = filteredObjects
                .Where(obj => obj is Entity ent && selectedLayers.Contains(ent.Layer))
                .ToList();
            // Validamos
            if (objetos == null || objetos.Count == 0)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"No objects found after filtering by type and layer in '{docName}'.\n\n" +
                    $"This file will not be processed."
                ).ShowDialog();
                // Finalizamos
                return null;
            }

            // return
            return objetos;
        }

        private static List<DBObject> get_ListValues_FromDicc(Dictionary<string, List<DBObject>> entitiesByType)
        {
            // Crear una lista para almacenar todos los valores
            List<DBObject> allValues = new List<DBObject>();

            // Recorrer el diccionario y agregar todos los valores a la lista
            foreach (var kvp in entitiesByType)
            {
                // Almacenamos
                allValues.AddRange(kvp.Value);
            }

            // return
            return allValues;
        }











    }
}


