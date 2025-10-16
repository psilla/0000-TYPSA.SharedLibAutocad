using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetBlockRefByNameFromXref
    {
        public static List<BlockReference> GetBlockRefByNameFromXref(
            List<BlockTableRecord> selectedXref,
            Transaction tr,
            string startsWith
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
                        // Obtenemos el BlockTableRecord
                        BlockTableRecord brBtr = tr.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                        // Obtenemos el nombre del bloque 
                        string blockName = brBtr.Name.Contains("|")
                            ? brBtr.Name.Split('|').Last()
                            : brBtr.Name;
                        // Validamos
                        if (blockName.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
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
                    $"selected Xrefs starting with \"{startsWith}\".",
                    "No Matches Found"
                );
            }

            // return
            return result;
        }





    }
}
