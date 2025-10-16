using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using TYPSA.SharedLib.Autocad.GetDocument;
//using TYPSA.SharedLib.Autocad.UserForms;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.DbObjectsByType
{
    public class cls_00_DbObjectsByType
    {
        public static Dictionary<string, List<DBObject>> dicc_DbObjects_ByType(
            Document doc
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
                        ((BlockTable)tr.
                            GetObject(doc.Database.BlockTableId, OpenMode.ForRead))[BlockTableRecord.ModelSpace],
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





    }
}


