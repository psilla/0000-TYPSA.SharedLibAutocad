using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetNestedBlockRef
    {
        public static List<BlockReference> GetAllNestedBlockRefs(
            Transaction tr,
            BlockReference blockRefAnfi
        )
        {
            List<BlockReference> nestedRefs = new List<BlockReference>();

            // Obtenemos BlockTableRecord
            BlockTableRecord skidBTR =
                tr.GetObject(blockRefAnfi.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            // Validamos
            if (skidBTR == null) return nestedRefs;

            // Iteramos
            foreach (ObjectId nestedId in skidBTR)
            {
                // Validamos como BlockReference
                if (tr.GetObject(nestedId, OpenMode.ForRead) is BlockReference nestedBr)
                {
                    // Almacenamos
                    nestedRefs.Add(nestedBr);
                }
            }

            // return
            return nestedRefs;
        }


    }
}
