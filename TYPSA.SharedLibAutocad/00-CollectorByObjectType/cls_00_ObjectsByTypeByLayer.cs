using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;
using TYPSA.SharedLib.Autocad.GetEntities;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.DbObjectsByType
{
    public static class SelectionModes
    {
        public const string Manual = "SelectManually";
        public const string All = "SelectAll";
        public const string Default = "UseDefault";
    }

    public class cls_00_ObjectsByTypeByLayer
    {
        private static List<DBObject> get_ListValues_FromDicc(
            Dictionary<string, List<DBObject>> entitiesByType
        )
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

        private static List<string> ResolveSelectionDrop(
            string fileName,
            HashSet<string> allNames,
            string itemLabel,
            bool askUser = true
        )
        {
            List<string> userSelection;
            // True
            if (askUser)
            {
                userSelection = InstanciarFormularios.CheckListBoxFormSearchOut(
                    $"Select the {itemLabel} to analyze in '{fileName}'.\n" +
                    $"Use Ctrl + A / Ctrl + D to Select / Deselect all.",
                    allNames.OrderBy(x => x).ToList()
                );
                // Validamos
                if (userSelection == null || userSelection.Count == 0)
                {
                    // Mensaje
                    new AutoCloseMessageForm(
                        $"No {itemLabel} were selected in '{fileName}'.\n\n" +
                        $"This file will not be processed."
                    ).ShowDialog();
                    // Finalizamos
                    return null;
                }
            }
            // False
            else
            {
                // Seleccionar todos
                userSelection = allNames.ToList();
            }
            // return
            return userSelection;
        }

        private static Dictionary<string, List<DBObject>> dicc_DbObjects_ByType(
            Document doc,
            bool includeNestedBlockRefs = true 
        )
        {
            // Diccionario para agrupar las entidades por su tipo
            Dictionary<string, List<DBObject>> entitiesByType =
                new Dictionary<string, List<DBObject>>();
            // Bloquear documento
            using (var dl = doc.LockDocument())
            {
                // Iniciar Transacción
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    // Obtener el espacio modelo
                    BlockTableRecord btr = tr.GetObject(
                        ((BlockTable)tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead))[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead
                    ) as BlockTableRecord;

                    // Iterar sobre los objetos en el espacio modelo
                    foreach (ObjectId id in btr)
                    {
                        // Validamos 
                        if (!id.IsValid) continue;

                        // Obtenemos el objeto
                        DBObject dbObject = tr.GetObject(id, OpenMode.ForRead);
                        // Validamos
                        if (dbObject == null) continue;

                        // Obtenemos nombre de tipo
                        string dbObjectType = dbObject.GetType().Name;

                        // Almacenamos el tipo como clave en caso de no existir aún, y una lista de valores
                        if (!entitiesByType.TryGetValue(dbObjectType, out List<DBObject> list))
                        {
                            list = new List<DBObject>();
                            entitiesByType[dbObjectType] = list;
                        }

                        // Añadimos a la lista el objeto
                        list.Add(dbObject);

                        // Si se requiere incluir BlockRef anidadas
                        if (includeNestedBlockRefs && dbObject is BlockReference blockRef)
                        {
                            // Validamos
                            if (blockRef == null) continue;

                            List<BlockReference> nestedRefs = cls_00_GetNestedBlockRef.GetAllNestedBlockRefs(
                                tr, blockRef
                            );
                            // Validamos
                            if (nestedRefs == null || nestedRefs.Count == 0) continue;

                            // Iteramos
                            foreach (BlockReference nestedRef in nestedRefs)
                            {
                                // Obtenemos el nombre
                                string nestedType = nestedRef.GetType().Name;
                                // Validamos
                                if (!entitiesByType.TryGetValue(nestedType, out List<DBObject> nestedList))
                                {
                                    nestedList = new List<DBObject>();
                                    entitiesByType[nestedType] = nestedList;
                                }
                                // Almacenamos
                                nestedList.Add(nestedRef);
                            }
                        }
                    }

                    // Cerramos transacción
                    tr.Commit();
                }
            }

            // Construir el mensaje para mostrar
            string message =
                $"Entities by Type found in '" +
                $"{cls_00_DocumentInfo.GetActiveDocumentName(doc)}':\n\n";
            // Iteramos
            foreach (var kvp in entitiesByType)
            {
                message += $"{kvp.Key}: {kvp.Value.Count} entities\n";
            }
            // Mostrar el mensaje
            new AutoCloseMessageForm(message).ShowDialog();

            // return
            return entitiesByType;
        }

        private static Dictionary<string, List<DBObject>> dicc_DbObjects_ByType_DataBase(
            Database db,
            string fileName,
            bool includeNestedBlockRefs = true
        )
        {
            Dictionary<string, List<DBObject>> entitiesByType =
                new Dictionary<string, List<DBObject>>();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt =
                    (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                BlockTableRecord btr =
                    (BlockTableRecord)tr.GetObject(
                        bt[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead
                    );

                foreach (ObjectId id in btr)
                {
                    if (!id.IsValid) continue;

                    DBObject dbObject = tr.GetObject(id, OpenMode.ForRead);
                    if (dbObject == null) continue;

                    string dbObjectType = dbObject.GetType().Name;

                    if (!entitiesByType.TryGetValue(dbObjectType, out List<DBObject> list))
                    {
                        list = new List<DBObject>();
                        entitiesByType[dbObjectType] = list;
                    }

                    list.Add(dbObject);

                    // Nested BlockRefs
                    if (includeNestedBlockRefs && dbObject is BlockReference blockRef)
                    {
                        List<BlockReference> nestedRefs =
                            cls_00_GetNestedBlockRef.GetAllNestedBlockRefs(tr, blockRef);

                        if (nestedRefs == null || nestedRefs.Count == 0) continue;

                        foreach (BlockReference nestedRef in nestedRefs)
                        {
                            string nestedType = nestedRef.GetType().Name;

                            if (!entitiesByType.TryGetValue(nestedType, out List<DBObject> nestedList))
                            {
                                nestedList = new List<DBObject>();
                                entitiesByType[nestedType] = nestedList;
                            }

                            nestedList.Add(nestedRef);
                        }
                    }
                }

                tr.Commit();
            }

            // Resumen
            string message =
                $"Entities by Type found in '{fileName}':\n\n";

            foreach (var kvp in entitiesByType)
            {
                message += $"{kvp.Key}: {kvp.Value.Count} entities\n";
            }

            new AutoCloseMessageForm(message).ShowDialog();

            // WIP
            MessageBox.Show(
                message.ToString(),
                "Entities Summary",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            // WIP

            return entitiesByType;
        }

        public static List<string> ResolveSelectionCheckList(
            string fileName,
            HashSet<string> allNames,
            string itemLabel,
            string selectionMode = SelectionModes.Manual,
            List<string> defaultItems = null
        )
        {
            List<string> userSelection;
            // Opcion 1
            if (selectionMode == SelectionModes.Manual)
            {
                // Form
                userSelection = InstanciarFormularios.CheckListBoxFormSearchOut(
                    $"Select the {itemLabel} to analyze in '{fileName}'.\n" +
                    $"Use Ctrl + A / Ctrl + D to Select / Deselect all.",
                    allNames.OrderBy(x => x).ToList()
                );
                // Validamos
                if (userSelection == null || userSelection.Count == 0)
                {
                    // Mensaje
                    new AutoCloseMessageForm(
                        $"No {itemLabel} were selected in " +
                        $"document '{fileName}'."
                    ).ShowDialog();
                    // Finalizamos
                    return null;
                }
            }
            // Opcion 2
            else if (selectionMode == SelectionModes.All)
            {
                // Seleccionar todos
                userSelection = allNames.ToList();
            }
            // Opcion 3
            else if (selectionMode == SelectionModes.Default)
            {
                // Opcion por defecto
                if (defaultItems == null || defaultItems.Count == 0) return null;
                // Obtenemos
                userSelection = defaultItems.Where(p => allNames.Contains(p)).ToList();
                // Validamos
                if (userSelection.Count == 0) return null;
            }
            else return null;

            // return
            return userSelection;
        }

        public static List<DBObject> get_DBObjectsByTypeByLayer_FromDicc(
            Document doc,
            string entitiesSelectionMode = SelectionModes.All,
            List<string> defaultEntities = null,
            string layerSelectionMode = SelectionModes.Manual,
            List<string> defaultLayers = null
        )
        {
            // Obtenemos el nombre del documento sin extensión
            string docName = cls_00_DocumentInfo.GetActiveDocumentName(doc);

            // Obtenemos el diccionario de Entidades
            Dictionary<string, List<DBObject>> diccEntities = dicc_DbObjects_ByType(doc, false);
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

            // =========================
            // SELECCION ENTIDADES
            // =========================

            HashSet<string> availableTypes = new HashSet<string>(diccEntities.Keys);
            // Seleccionamos Tipos de Entidad
            List<string> selectedTypes = ResolveSelectionCheckList(
                docName, availableTypes, "Entities", entitiesSelectionMode, defaultEntities
            );
            // Validamos
            if (selectedTypes == null) return null;

            // =========================
            // FILTRAMOS ENTIDADES
            // =========================

            // Filtrar diccionario por tipo
            Dictionary<string, List<DBObject>> diccEntitiesFiltered = diccEntities
                .Where(kvp => selectedTypes.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Extraer objetos combinados por tipo
            List<DBObject> filteredObjects = get_ListValues_FromDicc(diccEntitiesFiltered);
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

            // =========================
            // OBTENER CAPAS
            // =========================

            // Obtener capas únicas de esos objetos
            HashSet<string> allLayers = new HashSet<string>();
            // Iteramos
            foreach (var obj in filteredObjects.OfType<Entity>())
            {
                allLayers.Add(obj.Layer);
            }
            // Validamos
            if (allLayers.Count == 0) return null;

            // =========================
            // SELECCION CAPAS
            // =========================

            List<string> selectedLayers = ResolveSelectionCheckList(
                docName, allLayers, "Layers", layerSelectionMode, defaultLayers
            );
            // Validamos
            if (selectedLayers == null) return null;

            // =========================
            // FILTRAR POR CAPA
            // =========================

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















    }
}


