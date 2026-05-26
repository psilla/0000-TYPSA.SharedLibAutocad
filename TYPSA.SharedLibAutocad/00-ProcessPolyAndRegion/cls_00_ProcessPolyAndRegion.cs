using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.DeleteEntities;
using TYPSA.SharedLib.Autocad.IsolateEntities;
using TYPSA.SharedLib.Autocad.ProcessPoly;
using TYPSA.SharedLib.Autocad.ProcessRegion;
using TYPSA.SharedLib.UserForms;
using static TYPSA.SharedLib.Autocad.ProcessPoly.cls_00_ProcessOffsetPolyResult;
using static TYPSA.SharedLib.Autocad.ProcessPoly.cls_00_ProcessPolyResult;
using static TYPSA.SharedLib.Autocad.ProcessRegion.cls_00_ProcessRegionResult;

namespace TYPSA.SharedLib.Autocad.ProcessPolyAndRegion
{
    public class cls_00_ProcessPolyAndRegion
    {
        public static ProcessRegionResult ProcessPolysAndRegionsAsEnt(
            List<Entity> analyzePoly,
            Transaction tr,
            BlockTableRecord btr,
            Editor ed,
            dynamic tags,
            string projectUnits,
            double? offsetDistance = null
        )
        {
            // Validamos las poly
            ProcessPolyResult dataPoly = cls_00_ProcessAllPoly.ProcessAllPolyAsEnt(
                analyzePoly, tr, tags.SkidOutlineTag, projectUnits
            );

            // Acceso a las propiedades
            List<Polyline> validPoly = dataPoly.ValidPolylines;
            HashSet<ObjectId> polyToIsolate = dataPoly.PolylinesToIsolate;
            StringBuilder infoPoly = dataPoly.InfoSummary;
            int allPolyCount = dataPoly.Total;
            int nullPolyCount = dataPoly.NullCount;
            int validPolyCount = dataPoly.ValidCount;

            // Ver si existen poly nulas
            if (polyToIsolate.Count > 0)
            {
                // Mensaje
                ShowDiscardedAndIsolatedMessage(
                    tags.SkidOutlineTag, polyToIsolate.Count
                );
                // Aislamos los objetos
                cls_00_IsolateEntities.IsolateObjects(ed, polyToIsolate);
                // Cerramos transacción
                tr.Commit();
                // Finalizamos
                return null;
            }

            // Validamos offset por defecto
            if (!offsetDistance.HasValue)
            {
                // Form para introducir la distancia de offset
                double? offsetByUser = cls_00_InstaForm_TextBox.TextBoxFormOutAsDouble(
                    $"Enter the offset distance in ({projectUnits}) for {tags.SkidOutlineTag} " +
                    $"Polylines to see how many {tags.DcBlockTag} are contained within each one.",
                    "Distance selection form", 1
                );
                // Validamos
                if (!offsetByUser.HasValue)
                {
                    // Mensaje
                    MessageBox.Show(
                        "⚠ A valid value was not entered. The operation will be canceled.",
                        "Warning"
                    );
                    // Finalizamos
                    return null;
                }
                // Obtenemos la distancia como double
                offsetDistance = offsetByUser.Value;
            }

            // Obtenemos info de las poly desfasadas
            ProcessOffsetPolyResult dataOffsetPoly = cls_00_ProcessAllOffsetPoly.ProcessAllOffsetPoly(
                validPoly, tr, btr, tags.SkidOutlineTag, offsetDistance.Value
            );

            // Acceso a las propiedades
            List<Polyline> validOffsetPoly = dataOffsetPoly.ValidOffsetPolylines;
            List<Polyline> validOffsetPolyAndPoly = dataOffsetPoly.ValidOffsetAndOriginalPolys;
            HashSet<ObjectId> offsetPolyToIsolate = dataOffsetPoly.OffsetPolylinesToIsolate;
            Dictionary<Handle, Handle> dictPolyToOffsetPoly = dataOffsetPoly.DictPolyToOffset;
            StringBuilder infoOffsetPoly = dataOffsetPoly.InfoSummary;
            int allOffsetPolyCount = dataOffsetPoly.Total;
            int nullOffsetPolyCount = dataOffsetPoly.NullCount;
            int validOffsetPolyCount = dataOffsetPoly.ValidCount;

            // Ver si existen poly desfasadas nulas
            if (offsetPolyToIsolate.Count > 0)
            {
                // Borramos las polilíneas válidas desfasadas para evitar duplicados
                foreach (Polyline poly in validOffsetPolyAndPoly)
                {
                    // Validamos
                    if (poly == null) continue;
                    // Borrar la polilínea
                    cls_00_DeleteEntity.DeleteEntity(poly);
                }
                // Aislamos las que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToIsolate);
                // Cerramos transacción
                tr.Commit();
                // finalizamos
                return null;
            }

            // Obtenemos la info de las regiones procesadas
            ProcessRegionResult dataRegions = cls_00_ProcessAllRegion.ProcessAllRegion(
                validOffsetPolyAndPoly, validOffsetPoly, dictPolyToOffsetPoly,
                tr, btr, tags.SkidOutlineTag
            );

            // Acceder a los valores
            List<Region> validRegion = dataRegions.ValidRegions;
            HashSet<ObjectId> offsetPolyToRegionToIsolate = dataRegions.FailedRegionPolylines;
            Dictionary<Handle, Region> diccRegiones = dataRegions.HandleToRegion;
            Dictionary<Handle, Handle> dictPolyToRegion = dataRegions.PolyToRegionMap;
            StringBuilder infoOffsetPolyToRegion = dataRegions.InfoSummary;
            int allRegionCount = dataRegions.Total;
            int nullRegionCount = dataRegions.NullCount;
            int validRegionCount = dataRegions.ValidCount;

            // Ver si existen poly desfasadas que no se pudieron convertir en región
            if (offsetPolyToRegionToIsolate.Count > 0)
            {
                // Borrar las regiones válidas antes de aislar
                foreach (Region region in validRegion)
                {
                    // En caso de región válida
                    if (region != null)
                    {
                        // Borramos
                        cls_00_DeleteEntity.DeleteEntity(region);
                    }
                }
                // Aislar las polilíneas que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToRegionToIsolate);
                // Cerrar transacción
                tr.Commit();
                // Finalizamos
                return null;
            }
            // return
            return dataRegions;
        }

        public static ProcessRegionResult ProcessPolysAndRegions(
            PromptSelectionResult psrSkidOutline,
            Transaction tr,
            BlockTableRecord btr,
            Editor ed,
            dynamic tags,
            string projectUnits,
            double? offsetDistance = null
        )
        {
            // Validamos las poly
            ProcessPolyResult dataPoly = cls_00_ProcessAllPoly.ProcessAllPoly(
                psrSkidOutline.Value, tr, tags.SkidOutlineTag, projectUnits
            );

            // Acceso a las propiedades
            List<Polyline> validPoly = dataPoly.ValidPolylines;
            HashSet<ObjectId> polyToIsolate = dataPoly.PolylinesToIsolate;
            StringBuilder infoPoly = dataPoly.InfoSummary;
            int allPolyCount = dataPoly.Total;
            int nullPolyCount = dataPoly.NullCount;
            int validPolyCount = dataPoly.ValidCount;

            // Ver si existen poly nulas
            if (polyToIsolate.Count > 0)
            {
                // Mensaje
                ShowDiscardedAndIsolatedMessage(
                    tags.SkidOutlineTag, polyToIsolate.Count
                );
                // Aislamos los objetos
                cls_00_IsolateEntities.IsolateObjects(ed, polyToIsolate);
                // Cerramos transacción
                tr.Commit();
                // Finalizamos
                return null;
            }

            // Validamos offset por defecto
            if (!offsetDistance.HasValue)
            {
                // Form para introducir la distancia de offset
                double? offsetByUser = cls_00_InstaForm_TextBox.TextBoxFormOutAsDouble(
                    $"Enter the offset distance in ({projectUnits}) for {tags.SkidOutlineTag} " +
                    $"Polylines to see how many {tags.DcBlockTag} are contained within each one.",
                    "Distance selection form", 1
                );
                // Validamos
                if (!offsetByUser.HasValue)
                {
                    // Mensaje
                    MessageBox.Show(
                        "⚠ A valid value was not entered. The operation will be canceled.",
                        "Warning"
                    );
                    // Finalizamos
                    return null;
                }
                // Obtenemos la distancia como double
                offsetDistance = offsetByUser.Value;
            }

            // Obtenemos info de las poly desfasadas
            ProcessOffsetPolyResult dataOffsetPoly = cls_00_ProcessAllOffsetPoly.ProcessAllOffsetPoly(
                validPoly, tr, btr, tags.SkidOutlineTag, offsetDistance.Value
            );

            // Acceso a las propiedades
            List<Polyline> validOffsetPoly = dataOffsetPoly.ValidOffsetPolylines;
            List<Polyline> validOffsetPolyAndPoly = dataOffsetPoly.ValidOffsetAndOriginalPolys;
            HashSet<ObjectId> offsetPolyToIsolate = dataOffsetPoly.OffsetPolylinesToIsolate;
            Dictionary<Handle, Handle> dictPolyToOffsetPoly = dataOffsetPoly.DictPolyToOffset;
            StringBuilder infoOffsetPoly = dataOffsetPoly.InfoSummary;
            int allOffsetPolyCount = dataOffsetPoly.Total;
            int nullOffsetPolyCount = dataOffsetPoly.NullCount;
            int validOffsetPolyCount = dataOffsetPoly.ValidCount;

            // Ver si existen poly desfasadas nulas
            if (offsetPolyToIsolate.Count > 0)
            {
                // Borramos las polilíneas válidas desfasadas para evitar duplicados
                foreach (Polyline poly in validOffsetPolyAndPoly)
                {
                    // Validamos
                    if (poly == null) continue;
                    // Borrar la polilínea
                    cls_00_DeleteEntity.DeleteEntity(poly);
                }
                // Aislamos las que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToIsolate);
                // Cerramos transacción
                tr.Commit();
                // finalizamos
                return null;
            }

            // Obtenemos la info de las regiones procesadas
            ProcessRegionResult dataRegions = cls_00_ProcessAllRegion.ProcessAllRegion(
                validOffsetPolyAndPoly, validOffsetPoly, dictPolyToOffsetPoly,
                tr, btr, tags.SkidOutlineTag
            );

            // Acceder a los valores
            List<Region> validRegion = dataRegions.ValidRegions;
            HashSet<ObjectId> offsetPolyToRegionToIsolate = dataRegions.FailedRegionPolylines;
            Dictionary<Handle, Region> diccRegiones = dataRegions.HandleToRegion;
            Dictionary<Handle, Handle> dictPolyToRegion = dataRegions.PolyToRegionMap;
            StringBuilder infoOffsetPolyToRegion = dataRegions.InfoSummary;
            int allRegionCount = dataRegions.Total;
            int nullRegionCount = dataRegions.NullCount;
            int validRegionCount = dataRegions.ValidCount;

            // Ver si existen poly desfasadas que no se pudieron convertir en región
            if (offsetPolyToRegionToIsolate.Count > 0)
            {
                // Borrar las regiones válidas antes de aislar
                foreach (Region region in validRegion)
                {
                    // En caso de región válida
                    if (region != null)
                    {
                        // Borramos
                        cls_00_DeleteEntity.DeleteEntity(region);
                    }
                }
                // Aislar las polilíneas que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToRegionToIsolate);
                // Cerrar transacción
                tr.Commit();
                // Finalizamos
                return null;
            }
            // return
            return dataRegions;
        }

        public static ProcessRegionResult ProcessPolysAndRegions(
            SelectionSet selectionPolys,
            Transaction tr,
            BlockTableRecord btr,
            Editor ed,
            dynamic tags,
            string projectUnits,
            double? offsetDistance = null
        )
        {
            // Validamos las poly
            ProcessPolyResult dataPoly = cls_00_ProcessAllPoly.ProcessAllPoly(
                selectionPolys, tr, tags.SkidOutlineTag, projectUnits
            );

            // Acceso a las propiedades
            List<Polyline> validPoly = dataPoly.ValidPolylines;
            HashSet<ObjectId> polyToIsolate = dataPoly.PolylinesToIsolate;
            StringBuilder infoPoly = dataPoly.InfoSummary;
            int allPolyCount = dataPoly.Total;
            int nullPolyCount = dataPoly.NullCount;
            int validPolyCount = dataPoly.ValidCount;

            // Ver si existen poly nulas
            if (polyToIsolate.Count > 0)
            {
                // Mensaje
                ShowDiscardedAndIsolatedMessage(
                    tags.SkidOutlineTag, polyToIsolate.Count
                );
                // Aislamos los objetos
                cls_00_IsolateEntities.IsolateObjects(ed, polyToIsolate);
                // Cerramos transacción
                tr.Commit();
                // Finalizamos
                return null;
            }

            // Validamos offset por defecto
            if (!offsetDistance.HasValue)
            {
                // Form para introducir la distancia de offset
                double? offsetByUser = cls_00_InstaForm_TextBox.TextBoxFormOutAsDouble(
                    $"Enter the offset distance in ({projectUnits}) for {tags.SkidOutlineTag} " +
                    $"Polylines to see how many {tags.DcBlockTag} are contained within each one.",
                    "Distance selection form", 1
                );
                // Validamos
                if (!offsetByUser.HasValue)
                {
                    // Mensaje
                    MessageBox.Show(
                        "⚠ A valid value was not entered. The operation will be canceled.",
                        "Warning"
                    );
                    // Finalizamos
                    return null;
                }
                // Obtenemos la distancia como double
                offsetDistance = offsetByUser.Value;
            }

            // Obtenemos info de las poly desfasadas
            ProcessOffsetPolyResult dataOffsetPoly = cls_00_ProcessAllOffsetPoly.ProcessAllOffsetPoly(
                validPoly, tr, btr, tags.SkidOutlineTag, offsetDistance.Value
            );

            // Acceso a las propiedades
            List<Polyline> validOffsetPoly = dataOffsetPoly.ValidOffsetPolylines;
            List<Polyline> validOffsetPolyAndPoly = dataOffsetPoly.ValidOffsetAndOriginalPolys;
            HashSet<ObjectId> offsetPolyToIsolate = dataOffsetPoly.OffsetPolylinesToIsolate;
            Dictionary<Handle, Handle> dictPolyToOffsetPoly = dataOffsetPoly.DictPolyToOffset;
            StringBuilder infoOffsetPoly = dataOffsetPoly.InfoSummary;
            int allOffsetPolyCount = dataOffsetPoly.Total;
            int nullOffsetPolyCount = dataOffsetPoly.NullCount;
            int validOffsetPolyCount = dataOffsetPoly.ValidCount;

            // Ver si existen poly desfasadas nulas
            if (offsetPolyToIsolate.Count > 0)
            {
                // Borramos las polilíneas válidas desfasadas para evitar duplicados
                foreach (Polyline poly in validOffsetPolyAndPoly)
                {
                    // Validamos
                    if (poly == null) continue;
                    // Borrar la polilínea
                    cls_00_DeleteEntity.DeleteEntity(poly);
                }
                // Aislamos las que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToIsolate);
                // Cerramos transacción
                tr.Commit();
                // finalizamos
                return null;
            }

            // Obtenemos la info de las regiones procesadas
            ProcessRegionResult dataRegions = cls_00_ProcessAllRegion.ProcessAllRegion(
                validOffsetPolyAndPoly, validOffsetPoly, dictPolyToOffsetPoly,
                tr, btr, tags.SkidOutlineTag
            );

            // Acceder a los valores
            List<Region> validRegion = dataRegions.ValidRegions;
            HashSet<ObjectId> offsetPolyToRegionToIsolate = dataRegions.FailedRegionPolylines;
            Dictionary<Handle, Region> diccRegiones = dataRegions.HandleToRegion;
            Dictionary<Handle, Handle> dictPolyToRegion = dataRegions.PolyToRegionMap;
            StringBuilder infoOffsetPolyToRegion = dataRegions.InfoSummary;
            int allRegionCount = dataRegions.Total;
            int nullRegionCount = dataRegions.NullCount;
            int validRegionCount = dataRegions.ValidCount;

            // Ver si existen poly desfasadas que no se pudieron convertir en región
            if (offsetPolyToRegionToIsolate.Count > 0)
            {
                // Borrar las regiones válidas antes de aislar
                foreach (Region region in validRegion)
                {
                    // En caso de región válida
                    if (region != null)
                    {
                        // Borramos
                        cls_00_DeleteEntity.DeleteEntity(region);
                    }
                }
                // Aislar las polilíneas que fallaron
                cls_00_IsolateEntities.IsolateObjects(ed, offsetPolyToRegionToIsolate);
                // Cerrar transacción
                tr.Commit();
                // Finalizamos
                return null;
            }
            // return
            return dataRegions;
        }

        private static void ShowDiscardedAndIsolatedMessage(string nombreEntidad, int cantidad)
        {
            MessageBox.Show(
                $"{cantidad} {nombreEntidad} were discarded and will be isolated in AutoCAD.",
                $"Isolated {nombreEntidad}"
            );
        }



    }
}
