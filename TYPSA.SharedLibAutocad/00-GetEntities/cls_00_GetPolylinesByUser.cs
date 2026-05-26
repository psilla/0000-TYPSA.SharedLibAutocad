using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.Main;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetPolylinesByUser
    {
        public static void GetEntitiesByCustomSelection(
            Transaction tr,
            SelectionSet selection,
            List<Entity> entitiesToProcess
        )
        {
            // Iteramos
            foreach (SelectedObject selObj in selection)
            {
                // Obtenemos la entidad
                Entity ent = tr.GetObject(selObj.ObjectId, OpenMode.ForWrite, false, false) as Entity;
                // Validamos
                if (ent == null) continue;

                // Almacenamos
                entitiesToProcess.Add(ent);
            }
        }

        public static SelectionSet GetPolylinesByUser(
            Editor ed,
            bool allDocument,
            PromptSelectionResult psrSkidOutline,
            string skidOutlineTag
        )
        {
            // Definir variable para almacenar los contornos
            SelectionSet analyzePoly;
            // En caso de True
            if (allDocument)
            {
                // Directamente el SelectionSet
                analyzePoly = psrSkidOutline.Value;
            }
            // En caso de False
            else
            {
                EntityTypes entityTypes = EntityTypes.GetDefaultEntityTypes();
                // Hacemos nuestra selección personalizada
                PromptSelectionResult psr = cls_00_PromptSelectionWithFilter.PromptSelectionWithFilter(
                    ed, $"Select only the {skidOutlineTag} Polylines you want to analyze:",
                    new TypedValue((int)DxfCode.Start, entityTypes.Polyline)
                );
                // Validamos la seleccion
                if (psr.Status != PromptStatus.OK)
                {
                    // Mensaje
                    MessageBox.Show(
                        $"⚠ No {skidOutlineTag} were selected. The operation will be canceled.",
                        "Warning"
                    );
                    // Finalizamos
                    return null;
                }
                // Actualizamos la selección
                analyzePoly = psr.Value;
            }
            // return
            return analyzePoly;
        }

        public static SelectionSet GetPolylinesByUserByLayer(
            Editor ed,
            bool allDocument,
            PromptSelectionResult psrSkidOutline,
            string skidOutlineTag,
            string skidOutlineLayer
        )
        {
            EntityTypes entityTypes = EntityTypes.GetDefaultEntityTypes();

            // Definir variable para almacenar los contornos
            SelectionSet analyzePoly;
            // En caso de True
            if (allDocument)
            {
                // Directamente el SelectionSet
                analyzePoly = psrSkidOutline.Value;
            }
            // En caso de False
            else
            {
                // Definir filtro
                TypedValue[] filterValues = new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, entityTypes.Polyline),
                    new TypedValue((int)DxfCode.LayerName, skidOutlineLayer)
                };
                // Hacemos nuestra selección personalizada
                PromptSelectionResult psr = cls_00_PromptSelectionWithFilter.PromptSelectionWithFilter(
                    ed, $"Select only the {skidOutlineTag} Polylines you want to analyze:", filterValues
                );
                // Validamos la seleccion
                if (psr.Status != PromptStatus.OK)
                {
                    // Mensaje
                    MessageBox.Show(
                        $"⚠ No {skidOutlineTag} were selected. The operation will be canceled.",
                        "Warning"
                    );
                    // Finalizamos
                    return null;
                }
                // Actualizamos la selección
                analyzePoly = psr.Value;
            }
            // return
            return analyzePoly;
        }

        public static List<Entity> GetPolylinesByUserByLayerAsEnt(
            Transaction tr,
            Editor ed,
            bool allDocument,
            PromptSelectionResult psrSkidOutline,
            string selectionMessage,
            TypedValue[] filtros
        )
        {
            // Definir lista de entidades a procesar
            List<Entity> entitiesToProcess = new List<Entity>();

            // -----------------------------
            // Analizar todo el documento
            // -----------------------------

            if (allDocument)
            {
                GetEntitiesByCustomSelection(
                    tr, psrSkidOutline.Value, entitiesToProcess
                );
                // return
                return entitiesToProcess;
            }

            // -----------------------------
            // Seleccion personalizada
            // -----------------------------

            SelectionSet selection = cls_00_PromptSelectionWithFilter.GetSelectionSetByFilter(
                ed, selectionMessage, filtros
            );
            // Validamos
            if (selection == null) return null;

            // -----------------------------
            // Obtener entidades
            // -----------------------------

            GetEntitiesByCustomSelection(tr, selection, entitiesToProcess);
            // Validamos
            if (entitiesToProcess == null || entitiesToProcess.Count == 0) return null;

            // return
            return entitiesToProcess;
        }

        

        






    }
}
