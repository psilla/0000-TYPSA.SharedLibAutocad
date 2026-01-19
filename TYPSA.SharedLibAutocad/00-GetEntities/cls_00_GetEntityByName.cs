using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetEntityByName
    {
        public static PromptSelectionResult GetEntityByName(
            Editor ed,
            string entityTag,
            string entityType,
            string blockName
        )
        {
            // Definir el filtro por nombre de bloque y tipo de entidad
            var filtros = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, entityType),
                new TypedValue((int)DxfCode.BlockName, blockName)
            };

            // Seleccionamos
            PromptSelectionResult psr =
                cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(ed, filtros);
            // Validamos
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                // Mensaje
                MessageBox.Show(
                    $"No {entityTag} found.", "Warning"
                );
                // Finalizamos
                return null;
            }

            // return
            return psr;
        }

        public static PromptSelectionResult GetEntityByNames(
            Editor ed,
            string entityTag,
            string entityType,
            IEnumerable<string> blockNames
        )
        {
            // Definir el filtro por nombre de bloque y tipo de entidad
            var filterList = new List<TypedValue>
            {
                new TypedValue((int)DxfCode.Start, entityType)
            };

            filterList.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            // Admitir varios nombres de BlockRef
            foreach (string name in blockNames)
            {
                filterList.Add(new TypedValue((int)DxfCode.BlockName, name));
            }

            filterList.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            // Seleccionamos
            PromptSelectionResult psr =
                cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(
                    ed,
                    filterList.ToArray()
                );
            // Validamos
            if (psr == null || psr.Status != PromptStatus.OK)
            {
                // Mensaje
                MessageBox.Show(
                    $"No {entityTag} found.", "Warning"
                );
                // Finalizamos
                return null;
            }

            // return
            return psr;
        }
    }
}
