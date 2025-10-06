using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.SelectEntities;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetPolylinesByUser
    {
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
                // Hacemos nuestra selección personalizada
                PromptSelectionResult psr = cls_00_PromptSelectionWithFilter.PromptSelectionWithFilter(
                    ed, $"Select only the {skidOutlineTag} Polylines you want to analyze:",
                    new TypedValue((int)DxfCode.Start, "LWPOLYLINE")
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




    }
}
