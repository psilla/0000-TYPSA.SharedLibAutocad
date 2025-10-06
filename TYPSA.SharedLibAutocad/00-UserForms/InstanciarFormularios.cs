using System.Collections.Generic;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public class InstanciarFormularios
    {
        public static List<string> CheckListBoxFormOut(string mensaje, List<string> listInput)
        {
            // Crear instancia del formulario
            CheckListBoxForm ventana =
                new CheckListBoxForm(mensaje, listInput);

            // Mostrar el formulario de manera modal
            Application.Run(ventana);

            // Retornar la propiedad salida
            return ventana.salida;
        }

        public static object DropDownFormOut(string mensaje, bool? valorPorDefecto = null)
        {
            // Opciones para el ComboBox True/False
            object[] opciones = { true, false };

            // Crear instancia del formulario
            DropDownForm ventana =
                new DropDownForm(mensaje, opciones, valorPorDefecto);

            // Mostrar el formulario de manera modal
            System.Windows.Forms.Application.Run(ventana);

            // Retornar la propiedad salida
            return ventana.salida;
        }

        public static object DropDownFormOutWithFormText(string mensaje, string formText, bool? valorPorDefecto = null)
        {
            // Opciones para el ComboBox True/False
            object[] opciones = { true, false };

            // Crear instancia del formulario
            DropDownFormWithFormText ventana =
                new DropDownFormWithFormText(mensaje, opciones, formText, valorPorDefecto);

            // Mostrar el formulario de manera modal
            System.Windows.Forms.Application.Run(ventana);

            // Retornar la propiedad salida
            return ventana.salida;
        }

        public static string DropDownFormListOut(string mensaje, List<string> listInput, string formText, string defaultValue = null)
        {

            // Crear instancia del formulario
            DropDownFormList ventana =
                new DropDownFormList(mensaje, listInput, formText, defaultValue);

            // Mostrar el formulario de manera modal
            System.Windows.Forms.Application.Run(ventana);

            // Retornar la propiedad salida convertida a string o un valor predeterminado
            return ventana.salida as string ?? string.Empty;
        }

        public static string TextBoxFormOut(string mensaje, string defaultValue = "")
        {
            // Crear instancia del formulario con mensaje y valor por defecto
            TextBoxForm ventana = new TextBoxForm(mensaje, defaultValue);

            // Mostrar el formulario de manera modal
            System.Windows.Forms.Application.Run(ventana);

            // Retornar la propiedad salida
            return ventana.salida;
        }





    }
}
