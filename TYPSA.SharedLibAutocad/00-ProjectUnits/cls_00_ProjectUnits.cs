using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.GetDocument;
using TYPSA.SharedLib.UserForms;

namespace TYPSA.SharedLib.Autocad.ProjectUnits
{
    public class cls_00_ProjectUnits
    {
        public static string SetProjectUnits()
        {
            // Obtener diccionario completo de unidades
            Dictionary<string, int> unitsDict =
                cls_00_DocumentInfo.GetUnitsDictionary();

            // Mostrar formulario de selección
            string selectedUnit = InstanciarFormularios.DropDownFormListOut(
                "Select the new units for the project:",
                unitsDict.Keys.ToList(),
                "Selection form to configure the project units"

            );
            // Validamos
            if (string.IsNullOrEmpty(selectedUnit) || !unitsDict.ContainsKey(selectedUnit))
            {
                // Mensaje
                MessageBox.Show("No valid unit was selected. Operation cancelled.");
                // Finalizamos
                return null;
            }

            // Aplicar nueva unidad
            Autodesk.AutoCAD.ApplicationServices.Application.
                SetSystemVariable("INSUNITS", unitsDict[selectedUnit]);

            // Confirmación
            MessageBox.Show($"Units successfully changed to: {selectedUnit}");

            // Obtener nombre actualizado de unidades del proyecto
            return cls_00_DocumentInfo.GetDrawingUnitsName();
        }

        public static string GetAndSetProjectUnits()
        {
            // Obtener unidades actuales
            string currentUnits = cls_00_DocumentInfo.GetDrawingUnitsName();

            // Preguntar si quiere mantener o cambiar
            string message =
                $"True: Continue working with current project units ({currentUnits}).\n" +
                $"False: Change to other units selected by the user.";

            // Form
            object userChoice = InstanciarFormularios.DropDownFormOutWithFormText(
                message, "Project Units Selection Form", true
            );
            // Validamos
            if (!(userChoice is bool)) return null;

            // Obtenemos el boolean
            bool keepCurrentUnits = Convert.ToBoolean(userChoice);

            // Si elegimos mantener, finalizamos
            if (keepCurrentUnits) return currentUnits;

            // Si elegimos cambiar, mostramos lista y aplicamos
            return SetProjectUnits();
        }


    }
}
