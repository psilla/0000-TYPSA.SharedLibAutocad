using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Linq;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_SelectBlockRefByNameFromXref
    {
        public static bool SelectBlockRefByNameFromXref(
            BlockTableRecord xrefBTR,
            Transaction tr,
            string blockRefSkidNameXref,
            out HashSet<ObjectId> skidIds
        )
        {
            skidIds = null;

            // Obtenemos Skid por tipo de objeto y nombre de BlockRef
            List<BlockReference> psrSkidXref = cls_00_GetBlockRefByNameFromXref.GetBlockRefByNameFromXref(
                new List<BlockTableRecord> { xrefBTR }, tr, blockRefSkidNameXref
            );
            // Validamos
            if (psrSkidXref == null || psrSkidXref.Count == 0) return false;

            // Obtenemos los ids
            skidIds = new HashSet<ObjectId>(psrSkidXref.Select(br => br.ObjectId));

            // return
            return true;
        }
































    }
}
