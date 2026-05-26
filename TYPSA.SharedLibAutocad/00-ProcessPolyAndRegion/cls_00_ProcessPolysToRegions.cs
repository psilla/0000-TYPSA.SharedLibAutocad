using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.DeleteEntities;
using TYPSA.SharedLib.Autocad.IsolateEntities;
using TYPSA.SharedLib.Autocad.ProcessPoly;
using TYPSA.SharedLib.Autocad.ProcessRegion;
using static TYPSA.SharedLib.Autocad.ProcessPoly.cls_00_ProcessOffsetPolyResult;
using static TYPSA.SharedLib.Autocad.ProcessPoly.cls_00_ProcessPolyResult;
using static TYPSA.SharedLib.Autocad.ProcessRegion.cls_00_ProcessRegionResult;

namespace TYPSA.SharedLib.Autocad.ProcessPolyAndRegion
{
    public class cls_00_ProcessPolysToRegions
    {
        public static bool ProcessPolysToRegions(
            Editor ed,
            Transaction tr,
            BlockTableRecord btr,
            SelectionSet analyzePoly,
            string entityTag,
            double offsetDistance,
            string projectUnits,
            out List<Region> validRegion,
            out Dictionary<Handle, Handle> dictPolyToRegion
        )
        {
            validRegion = new List<Region>();
            dictPolyToRegion = new Dictionary<Handle, Handle>();

            // ==========================
            // 1. Procesamos Polys
            // ==========================

            ProcessPolyResult dataPolys = cls_00_ProcessAllPoly.ProcessAllPoly(
                analyzePoly, tr, entityTag, projectUnits
            );
            // Accedemos a las propiedades
            List<Polyline> validPolys = dataPolys.ValidPolylines;
            HashSet<ObjectId> polyToIsolate = dataPolys.PolylinesToIsolate;
            // Validamos
            if (polyToIsolate.Count > 0)
            {
                // Mensaje
                MessageBox.Show(
                    $"{polyToIsolate.Count} {entityTag} were discarded " +
                    $"and will be isolated in AutoCAD.",
                    $"Isolated Contornos Generales"
                );
                // Aislamos los objetos
                cls_00_IsolateEntities.IsolateObjects(ed, polyToIsolate);
                // Finalizamos
                return false;
            }

            // ==========================
            // 2. Procesamos Polys desfasadas
            // ==========================

            ProcessOffsetPolyResult dataOffsetPolys = cls_00_ProcessAllOffsetPoly.ProcessAllOffsetPoly(
                validPolys, tr, btr, entityTag, offsetDistance
            );
            // Accedemos a las propiedades
            List<Polyline> validOffsetPolys = dataOffsetPolys.ValidOffsetPolylines;
            List<Polyline> validOffsetPolysAndPolys = dataOffsetPolys.ValidOffsetAndOriginalPolys;
            HashSet<ObjectId> offsetPolyToIsolate = dataOffsetPolys.OffsetPolylinesToIsolate;
            Dictionary<Handle, Handle> dictPolyToOffsetPoly = dataOffsetPolys.DictPolyToOffset;
            // Validamos
            if (offsetPolyToIsolate.Count > 0)
            {
                // Borramos las polilíneas válidas desfasadas para evitar duplicados
                foreach (Polyline poly in validOffsetPolysAndPolys)
                {
                    // Validamos
                    if (poly == null) continue;
                    // Borrar la polilínea
                    cls_00_DeleteEntity.DeleteEntity(poly);
                }
                // Aislamos las que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToIsolate);
                // Finalizamos
                return false;
            }

            // ==========================
            // 3. Procesamos regiones
            // ==========================

            ProcessRegionResult dataRegions = cls_00_ProcessAllRegion.ProcessAllRegion(
                validOffsetPolysAndPolys, validOffsetPolys, dictPolyToOffsetPoly,
                tr, btr, entityTag
            );
            // Accedemos a los valores
            validRegion = dataRegions.ValidRegions;
            HashSet<ObjectId> offsetPolyToRegionToIsolate = dataRegions.FailedRegionPolylines;
            dictPolyToRegion = dataRegions.PolyToRegionMap;
            // Validamos
            if (offsetPolyToRegionToIsolate.Count > 0)
            {
                // Borrar las regiones válidas antes de aislar
                foreach (Region region in validRegion)
                {
                    if (region != null)
                    {
                        cls_00_DeleteEntity.DeleteEntity(region);
                    }
                }
                // Aislar las polilíneas que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToRegionToIsolate);
                // Finalizamos
                return false;
            }
            // return
            return true;
        }
    }
}
