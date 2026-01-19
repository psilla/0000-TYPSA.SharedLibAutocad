using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetBlockRefByLayerFromXref
    {
        public static bool GetBlockRefIdsByLayerFromXref(
            BlockTableRecord xrefBTR,
            Transaction tr,
            string layerName,
            out HashSet<ObjectId> skidIds
        )
        {
            skidIds = null;
            // Obtenemos por tipo de objeto y capa de BlockRef
            List<BlockReference> psrEntXref = GetBlockRefByLayerFromXref(
                new List<BlockTableRecord> { xrefBTR }, tr, layerName
            );
            // Validamos
            if (psrEntXref == null || psrEntXref.Count == 0) return false;

            // Obtenemos los ids
            skidIds = new HashSet<ObjectId>(psrEntXref.Select(br => br.ObjectId));

            // return
            return true;
        }

        public static List<BlockReference> GetBlockRefByLayerFromXref(
            List<BlockTableRecord> selectedXref,
            Transaction tr,
            string layerName,
            bool acceptWithoutPrefix = true // compara contra la parte tras el '|'
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
                    $"No BlockReference elements were found inside the " +
                    $"selected Xrefs on layer \"{layerName}\".",
                    "No Matches Found"
                );
            }

            // return
            return result;
        }


    }
}
