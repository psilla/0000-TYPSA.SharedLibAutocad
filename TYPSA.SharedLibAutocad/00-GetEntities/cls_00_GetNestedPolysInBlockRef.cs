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
            // Accedemos al btr 
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

        public static List<Polyline> GetNestedPolysInBlockRef(
            BlockReference blockRef,
            Transaction tr,
            List<string> polyLayers
        )
        {
            List<Polyline> polyList = new List<Polyline>();
            // Validamos
            if (blockRef == null) return polyList;

            // HashSet layers
            HashSet<string> validLayers = new HashSet<string>(
                polyLayers ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase
            );

            // Accedemos al btr 
            BlockTableRecord btr =
                tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            // Validamos
            if (btr == null) return polyList;

            // Iteramos entidades
            foreach (ObjectId entId in btr)
            {
                // Obtener entidad
                Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                // Validamos poly
                if (!(ent is Polyline poly)) continue;

                // Validamos layer
                if (validLayers.Contains(poly.Layer))
                {
                    // Almacenamos
                    polyList.Add(poly);
                }
            }

            // return
            return polyList;
        }


    }
}
