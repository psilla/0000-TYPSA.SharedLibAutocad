using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;

namespace TYPSA.SharedLibAutocad._00_GetEntities
{
    public class cls_00_GetBlockRefByLayerFromXref
    {
        public static List<BlockReference> GetBlockRefByLayerFromXref(
            List<BlockTableRecord> selectedXref,
            Transaction tr,
            string layerName,
            bool acceptWithoutPrefix = true // si true, compara también contra la parte tras el '|'
        )
        {
            // Definimos lista vacía
            List<BlockReference> result = new List<BlockReference>();

            // Recorremos las Xrefs seleccionadas por el usuario
            foreach (BlockTableRecord xrefBtr in selectedXref)
            {
                // Validamos que sea una Xref 
                if (!xrefBtr.IsFromExternalReference) continue;

                // Recorremos las entidades dentro de la Xref
                foreach (ObjectId entId in xrefBtr)
                {
                    // Obtenemos la entidad
                    Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                    // Validamos que sea BlockReference
                    if (ent is BlockReference br)
                    {
                        // Obtenemos la capa
                        string entLayer = br.Layer ?? string.Empty;

                        // Coincidencia exacta (con posible prefijo XREF|)
                        bool match = entLayer.Equals(layerName, StringComparison.OrdinalIgnoreCase);
                        // Si no coincide y permitimos comparar sin prefijo, quitar "XREF|" si existe
                        if (!match && acceptWithoutPrefix && entLayer.IndexOf('|') >= 0)
                        {
                            // Redefinimos el nombre de la capa
                            int lastBar = entLayer.LastIndexOf('|');
                            string leaf = lastBar >= 0 ? entLayer.Substring(lastBar + 1) : entLayer;
                            match = leaf.Equals(layerName, StringComparison.OrdinalIgnoreCase);
                        }
                        // Validamos
                        if (match)
                        {
                            // Almacenamos
                            result.Add(br);
                        }
                    }
                }
            }
            // Validamos
            if (result.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    $"No BlockReference elements with Property Sets applied were found inside the " +
                    $"selected Xrefs on layer \"{layerName}\".",
                    "No Matches Found"
                );
            }

            // return
            return result;
        }


    }
}
