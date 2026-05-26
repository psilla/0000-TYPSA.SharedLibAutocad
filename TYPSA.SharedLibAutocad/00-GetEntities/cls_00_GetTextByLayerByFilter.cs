using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetLayersInfo;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetTextByLayerByFilter
    {
        public static List<Entity> GetTextByLayerAndFilter(
            BlockTable bt,
            Transaction tr,
            List<string> layers,
            string entityTag,
            List<Func<string, bool>> filters = null,
            string layerNameByDefault = null
        )
        {
            // Pedimos la capa al usuario
            string selectedLayer = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                layers, entityTag, layerNameByDefault
            );
            // Validamos
            if (selectedLayer == null) return null;

            // Buscar entidades de texto en la capa seleccionada que cumplan las condiciones
            List<Entity> label = TryGetTextByLayerAndFilter(
                bt, tr, filters, selectedLayer
            );
            // Validamos
            if (label == null || label.Count == 0)
            {
                // Mensaje
                MessageBox.Show($"⚠ No {entityTag} found. Operation cancelled.", "Warning");
                // Finalizamos
                return null;
            }
            // return
            return label;
        }

        public static List<Entity> GetTextByLayerAndFilter(
            BlockTable bt,
            Transaction tr,
            string entityTag,
            string selectedLayer,
            List<Func<string, bool>> filters = null
        )
        {
            // Buscar entidades de texto en la capa seleccionada que cumplan las condiciones
            List<Entity> label = TryGetTextByLayerAndFilter(
                bt, tr, filters, selectedLayer
            );
            // Validamos
            if (label == null || label.Count == 0)
            {
                // Mensaje
                MessageBox.Show($"⚠ No {entityTag} found. Operation cancelled.", "Warning");
                // Finalizamos
                return null;
            }
            // return
            return label;
        }

        public static List<Entity> TryGetTextByLayerAndFilter(
            BlockTable bt,
            Transaction tr,
            IEnumerable<Func<string, bool>> filters = null,
            string selectedLayer = null
        )
        {
            List<Entity> matchingTexts = new List<Entity>();
            // Accedemos al espacio modelo
            BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
            // Iteramos
            foreach (ObjectId id in modelSpace)
            {
                // Validamos
                if (!(tr.GetObject(id, OpenMode.ForRead) is Entity ent)) continue;

                // Validamos entidades en capa
                if (!string.Equals(ent.Layer, selectedLayer, StringComparison.OrdinalIgnoreCase)) continue;

                string content = null;
                // Obtenemos el contenido del texto
                if (ent is MText mt)
                    content = mt.Text?.Trim();
                else if (ent is DBText db)
                    content = db.TextString?.Trim();
                // Validamos
                if (string.IsNullOrWhiteSpace(content)) continue;

                // Aplicamos filtros si existen
                if (filters != null && !filters.All(f => f(content))) continue;

                // Añadimos
                matchingTexts.Add(ent);
            }
            // return
            return matchingTexts;
        }
    }
}
