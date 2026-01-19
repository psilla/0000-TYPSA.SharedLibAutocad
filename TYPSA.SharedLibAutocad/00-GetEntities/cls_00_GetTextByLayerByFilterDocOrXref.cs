using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using TYPSA.SharedLib.Autocad.GetLayersInfo;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetTextByLayerByFilterDocOrXref
    {
        public static bool GetTextByLayerAndFilterDocOrXref(
            bool isXrefDocument,
            Transaction tr,
            BlockTableRecord xrefBtr,
            BlockTable bt,
            List<string> layersDocOrXref,
            string entityTag,
            out HashSet<ObjectId> entIds,
            string layerNameByDefault = null,
            IEnumerable<Func<string, bool>> filters = null
        )
        {
            entIds = null;
            // Xref
            if (isXrefDocument)
            {
                // Pedimos al usuario seleccionar la capa
                string selectedLayer = cls_00_AskLayerNameFromUser.AskLayerNameFromUser(
                    layersDocOrXref, entityTag, layerNameByDefault
                );
                // Validamos
                if (selectedLayer == null) return false;

                // Obtenemos los ids
                bool entityBool = cls_00_GetTextDbByLayerFromXref.GetTextDbByLayerFromXref(
                    tr, layersDocOrXref, xrefBtr, selectedLayer, out entIds
                );
                // return
                return entityBool;
            }
            // Active Document
            else
            {
                // Obtenemos las entidades
                List<Entity> ent = cls_00_GetTextByLayerByFilter.GetTextByLayerAndFilter(
                    bt, tr, layersDocOrXref, entityTag, filters?.ToList(), layerNameByDefault
                );
                // Validamos
                if (ent == null) return false;

                // Obtenemos los ids
                entIds = ent.Select(e => e.ObjectId).ToHashSet();
                // return
                return true;
            }
        }


    }
}
