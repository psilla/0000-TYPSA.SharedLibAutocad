using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.ProcessRegion
{
    public class cls_00_GetOutRegionFromBlockRef
    {
        public static Polyline GetOuterPolyFromBlockRef(
            BlockReference blkRef,
            Transaction tr,
            BlockTableRecord btr,
            Dictionary<Handle, string> failedPoly
        )
        {
            // Validamos entrada
            if (blkRef == null) return null;

            // Obtener la definición del bloque
            BlockTableRecord btrDef = tr.GetObject(blkRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            // Validamos
            if (btrDef == null) return null;

            // Lista para guardar las polilíneas válidas
            List<Polyline> polylines = new List<Polyline>();

            // Recorrer entidades dentro del bloque
            foreach (ObjectId entId in btrDef)
            {
                Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                // Validamos poly
                if (ent is Polyline poly)
                    // Almacenamos
                    polylines.Add(poly);
            }
            // Validamos
            if (polylines.Count == 0)
            {
                // Mostramos
                failedPoly[blkRef.Handle] = "No polylines found inside the BlockReference.";
                // Finalizamos
                return null;
            }

            // Buscar la polilínea con mayor área → se asume exterior
            Polyline outerPoly = polylines
                .Where(p => p.Closed)
                .OrderByDescending(p => GetPolylineAreaSafe(p))
                .FirstOrDefault();
            // Validamos
            if (outerPoly == null)
            {
                // Mostramos
                failedPoly[blkRef.Handle] = "No closed outer polyline found.";
                // Finalizamos
                return null;
            }

            // Clonamos la polilínea al espacio actual (para poder convertirla)
            Polyline clonedPoly = outerPoly.Clone() as Polyline;
            // Validamos
            if (clonedPoly == null)
            {
                // Mostramos
                failedPoly[blkRef.Handle] = "Failed to clone polyline.";
                // Finalizamos
                return null;
            }

            // Aplicamos la transformación del bloque
            clonedPoly.TransformBy(blkRef.BlockTransform);

            // Agregar a la BlockTableRecord
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(clonedPoly, btr, tr);

            // return
            return clonedPoly;
        }

        public static Region GetOuterRegionFromBlockRef(
            BlockReference blkRef,
            Transaction tr,
            BlockTableRecord btr,
            Dictionary<Handle, string> failedPoly,
            Dictionary<Handle, Region> diccRegiones
        )
        {
            // Obtener la polilínea exterior clonada
            Polyline clonedPoly = 
                GetOuterPolyFromBlockRef(blkRef, tr, btr, failedPoly);
            // Validamos
            if (clonedPoly == null) return null;

            // Convertimos a region
            Region region = cls_00_ConvertPolyToRegion.
                ConvertPolyToRegion(clonedPoly, tr, btr, failedPoly, diccRegiones);
            // Validamos
            if (region == null) return null;

            // Borramos la poly clonada
            // try
            try
            {
                clonedPoly.UpgradeOpen();
                clonedPoly.Erase();
            }
            // catch
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                failedPoly[blkRef.Handle] = $"⚠ Failed to delete cloned polyline: {ex.Message}";
            }
            // return
            return region;
        }

        private static double GetPolylineAreaSafe(Polyline poly)
        {
            try { return poly.Area; }
            catch { return 0.0; }
        }


    }
}
