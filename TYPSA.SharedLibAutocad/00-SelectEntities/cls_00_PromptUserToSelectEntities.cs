using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.SelectEntities
{
    public class cls_00_PromptUserToSelectEntities
    {
        public static List<ObjectId> PromptUserToSelectEntities(
            Editor ed,
            Transaction tr,
            int missingCount,
            string entityTag
        )
        {
            List<ObjectId> selectedIds = new List<ObjectId>();

            // Definir el filtro 
            TypedValue[] filterValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Operator, "<OR"),
                    new TypedValue((int)DxfCode.Start, "INSERT"),
                    new TypedValue((int)DxfCode.Start, "TEXT"),
                    new TypedValue((int)DxfCode.Start, "MTEXT"),
                new TypedValue((int)DxfCode.Operator, "OR>")
            };
            SelectionFilter filter = new SelectionFilter(filterValues);

            // Configuramos el prompt
            PromptSelectionOptions pso = new PromptSelectionOptions
            {
                MessageForAdding = $"Select {entityTag} to associate...",
                AllowDuplicates = false
            };

            // Limpia selección previa
            ed.SetImpliedSelection(new ObjectId[0]);
            // Iteramos
            while (selectedIds.Count < missingCount)
            {
                // Solicitamos seleccion
                PromptSelectionResult psr = ed.GetSelection(pso, filter);
                // Validamos
                if (psr.Status != PromptStatus.OK || psr.Value.Count == 0)
                {
                    // Mensaje
                    MessageBox.Show(
                        $"⚠ You must select at least one {entityTag}.",
                        "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    // Obviamos
                    continue;
                }
                // Iteramos
                foreach (SelectedObject selObj in psr.Value)
                {
                    // Obviamos
                    if (selObj == null) continue;
                    // Obtenemos objeto
                    DBObject obj = tr.GetObject(selObj.ObjectId, OpenMode.ForRead);
                    // Validamos
                    if (obj is BlockReference || obj is DBText || obj is MText)
                    {
                        // Validamos
                        if (!selectedIds.Contains(selObj.ObjectId))
                        {
                            // Añadimos
                            selectedIds.Add(selObj.ObjectId);
                            // Paramos
                            if (selectedIds.Count == missingCount) break;
                        }
                    }
                    else
                    {
                        // Mensaje
                        MessageBox.Show(
                            $"❌ Invalid object selected: {obj.GetType().Name}\n" +
                            $"Only BlockReference, DBText, or MText objects are allowed.\nPlease select a valid one.",
                            "Invalid Selection",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                // Validamos
                if (selectedIds.Count < missingCount)
                {
                    // Mensaje
                    MessageBox.Show(
                        $"You still need to select {missingCount - selectedIds.Count} valid {entityTag}.",
                        "Incomplete Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            // Aplicar la seleccion final al editor
            ed.SetImpliedSelection(selectedIds.ToArray());
            // return
            return selectedIds;
        }

    }
}
