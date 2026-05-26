using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.Main;

namespace TYPSA.SharedLib.Autocad.SelectEntities
{
    public class cls_00_PromptUserToSelectEntitiesFree
    {
        public static List<ObjectId> PromptUserToSelectEntitiesFree(
            Editor ed,
            Transaction tr,
            string entityTag
        )
        {
            EntityTypes entityTypes = EntityTypes.GetDefaultEntityTypes();

            List<ObjectId> selectedIds = new List<ObjectId>();
            // Definir el filtro 
            TypedValue[] filterValues = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Operator, "<OR"),
                    new TypedValue((int)DxfCode.Start, entityTypes.BlockReference),
                    new TypedValue((int)DxfCode.Start, entityTypes.Text),
                    new TypedValue((int)DxfCode.Start, entityTypes.MText),
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

            // Solicitamos seleccion
            PromptSelectionResult psr = ed.GetSelection(pso, filter);
            // Validamos
            if (psr.Status != PromptStatus.OK || psr.Value.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    $"⚠ No {entityTag} selected. Operation canceled.",
                    "Selection Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                // Finalizamos
                return selectedIds;
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
                    // Añadimos
                    selectedIds.Add(selObj.ObjectId);
                }
                else
                {
                    // Mensaje
                    MessageBox.Show(
                        $"❌ Invalid object selected: {obj.GetType().Name}\n" +
                        $"Only BlockReference, DBText, or MText objects are allowed.",
                        "Invalid Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
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
