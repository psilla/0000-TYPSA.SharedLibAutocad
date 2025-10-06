using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace TYPSA.SharedLib.Autocad.ShowInfoBox
{
    public class cls_00_ShowInfoBox
    {

        public static void ShowStringBuilder(
            string titulo,
            string contenido
        )
        {
            // 🔹 Normalizar saltos de línea
            contenido = contenido.Replace("\n", Environment.NewLine);

            // 🔹 Obtener el tamaño de la pantalla
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // 🔹 Crear el formulario
            Form form = new Form();
            form.Text = titulo;
            form.StartPosition = FormStartPosition.CenterScreen; // Centrar el formulario
            form.Size = new System.Drawing.Size(800, screenHeight / 2); // Ancho fijo, altura = mitad de la pantalla
            form.MinimumSize = new System.Drawing.Size(500, 300); // Tamaño mínimo para evitar colapsos
            form.BackColor = Color.White;

            // 🔹 Crear el TextBox
            TextBox textBox = new TextBox();
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.ReadOnly = true;
            textBox.Font = new System.Drawing.Font("Segoe UI", 10);
            textBox.Dock = DockStyle.Fill;
            textBox.Text = contenido; // Asignar contenido con saltos de línea normalizados

            // Calcular ancho máximo del contenido
            int maxContentWidth;
            using (Graphics g = form.CreateGraphics())
            {
                maxContentWidth = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                           .Max(line => (int)g.MeasureString(line, textBox.Font).Width);
            }

            // Calcular ancho del título
            // Damos un margen por los botones de minimizar, maximizar y cerrar que solapan el título
            int margen = 500;
            int titleWidth = TextRenderer.MeasureText(titulo, SystemFonts.CaptionFont).Width + margen;

            // Tomar el mayor de los dos, con un margen adicional
            int finalWidth = Math.Min(Math.Max(maxContentWidth, titleWidth) + 50, screenWidth - 100);
            form.Width = finalWidth;

            // 🔹 Crear botón de cierre
            Button closeButton = new Button();
            closeButton.Text = "Cerrar";
            closeButton.Dock = DockStyle.Bottom;
            closeButton.Height = 40;
            closeButton.Click += (sender, e) => form.Close();

            // 💡 Hacer que ENTER active el botón de cierre
            form.AcceptButton = closeButton;

            // 🔹 Agregar controles al formulario
            form.Controls.Add(textBox);
            form.Controls.Add(closeButton);

            // 🔹 Mostrar el formulario
            form.ShowDialog();
        }










    }
}


