using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetNestedPolysInBlockRef
    {
        public static List<Polyline> GetNestedPolysInBlockRef(
            BlockReference blockRef,
            Transaction tr,
            string polyLayer
        )
        {
            List<Polyline> polyList = new List<Polyline>();
            // Accedemos al bloque del tracker
            BlockTableRecord btr =
                tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            // Validamos
            if (btr != null)
            {
                // Iteramos
                foreach (ObjectId entId in btr)
                {
                    // Buscamos poly 
                    if (tr.GetObject(entId, OpenMode.ForRead) is Polyline poly)
                    {
                        // Solo aceptar si la capa coincide
                        if (poly.Layer.Equals(polyLayer, StringComparison.OrdinalIgnoreCase))
                        {
                            // Almacenamos
                            polyList.Add(poly);
                        }
                    }
                }
            }
            // return
            return polyList;
        }


    }
}
