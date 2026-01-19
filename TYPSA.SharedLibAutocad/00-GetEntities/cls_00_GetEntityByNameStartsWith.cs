using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetEntityByNameStartsWith
    {
        public static PromptSelectionResult GetEntityByNameStartsWith(
            Editor ed,
            string entityTag,
            string entityType,
            string startsWith
        )
        {
            // Definir el filtro por nombre de bloque y tipo de entidad
            var filtros = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, entityType),
                new TypedValue((int)DxfCode.BlockName, startsWith)
            };

            // Realizar la selección
            PromptSelectionResult psr =
                cls_00_GetAllSelectionByFilter.GetAllSelectionByFilter(ed, filtros);

            // Verificación
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
