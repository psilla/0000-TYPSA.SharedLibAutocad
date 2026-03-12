using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System;


namespace TYPSA.SharedLib.Autocad.GetAttribute
{
    public class cls_07_GetAttributesFromEntity
    {
        public static string GetAttributeValueFromBlockReference(
            BlockReference blockRef,
            string attrName,
            Transaction tr
        )
        {
            // Validamos
            if (blockRef == null || blockRef.AttributeCollection.Count == 0) return null;

            // Lista de etiquetas encontradas
            List<string> availableTags = new List<string>();

            // Recorremos atributos
            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                // Validamos
                if (attId.IsErased) continue;

                // Obtenemos el atributo
                AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                // Validamos
                if (attRef == null) continue;

                // Almacenamos
                availableTags.Add(attRef.Tag);
                // Validamos
                if (attRef.Tag.Equals(attrName, StringComparison.OrdinalIgnoreCase))
                    // return
                    return attRef.TextString.Trim();
            }

            // Si llegamos aquí, el atributo no fue encontrado
            string availableList = availableTags.Count > 0
                ? string.Join(", ", availableTags)
                : "(No attributes found)";

            // Finalizamos
            return null;
        }

        















    }
}
