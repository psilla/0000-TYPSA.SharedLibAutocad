using System;
using System.Windows.Forms;
using System.Drawing;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public class Clases : Form
    {
        public static Point get_location_label_textBox(Size formSize, Size controlSize, int spacing)
        {
            // Calcular la posición X
            int x = spacing;

            // Calcular la posición Y centrada verticalmente
            int y = (formSize.Height - controlSize.Height) / 2;

            // Retornar la ubicación como un Point
            return new Point(x, y);
        }

        public static Point get_location_label_button(Size formSize, Size controlSize, int spacing)
        {
            // Cálculo para colocar el Label en 1/5 de la altura de la ventana
            int x = spacing;
            int y = (formSize.Height - controlSize.Height) / 5;

            return new Point(x, y);
        }

        public static Point get_location_textbox(Label label, Size formSize, Size controlSize)
        {
            // Coordenada donde termina el Label
            int x = label.Location.X + label.Width;

            // Coordenada Y centrada verticalmente
            int y = (formSize.Height - controlSize.Height) / 2;

            // Retornar la posición del TextBox
            return new Point(x, y);
        }

        public static int get__width_textbox(Label label, int uiWidth, int spacing)
        {
            // Coordenada donde termina el Label
            int ptoIni = label.Location.X + label.Width;

            // Punto final de la ventana restando el margen
            int ptoFin = uiWidth - spacing;

            // Retornar el ancho del TextBox
            return ptoFin - ptoIni;
        }

        public static Point get_location_button(Size formSize, Size controlSize)
        {
            // Calcular la posición para centrar el control en la ventana
            int x = (formSize.Width - controlSize.Width) / 2;
            int y = (formSize.Height - controlSize.Height) / 2;
            return new Point(x, y);
        }

        public static Point centrar_Formulario(Rectangle screenSize, int formWidth, int formHeight)
        {
            // Calcular la posición de la ventana
            int x = (screenSize.Width - formWidth) / 2;
            int y = (screenSize.Height - formHeight) / 2;

            // Establecer la posición de la ventana
            return new Point(x, y);
        }

        public static Label label_Header(string mensajeSel, int spacing)
        {
            // Crear el Label
            Label labelSel = new Label();

            // Estilo de texto para el encabezado
            Font font = new Font("Helvetica", 8, FontStyle.Bold);
            labelSel.Font = font;
            labelSel.Text = mensajeSel;

            // Configurar la ubicación y tamaño
            labelSel.Location = new Point(spacing, spacing);
            labelSel.AutoSize = true; // Permitir que el tamaño del label se adapte al texto

            return labelSel;
        }

        public static Label label_TextBox(Size formSize, int spacing)
        {
            // Crear el Label
            Label label = new Label();

            // Configurar propiedades del Label
            label.Font = new Font("Helvetica", 8);
            label.Text = "Enter a value:";
            label.AutoSize = true; // Permitir que el tamaño del Label se adapte al texto

            // Calcular ubicación usando GetLocationLabelTextBox
            label.Location = get_location_label_textBox(formSize, label.PreferredSize, spacing);

            return label;
        }

        public static TextBox textBox(int uiWidth, int spacing, Label label, Size formSize)
        {
            // Crear y configurar el TextBox
            TextBox textBox = new TextBox
            {
                BackColor = Color.FromArgb(245, 245, 245), // Gris más claro, estilo moderno
                BorderStyle = BorderStyle.FixedSingle,     // Borde fino
                Font = new Font("Segoe UI", 9),            // Fuente moderna y estándar
                Size = new Size(get__width_textbox(label, uiWidth, spacing), 50), // Establecer tamaño
                Location = get_location_textbox(label, formSize, label.PreferredSize) // Calcular ubicación
            };

            return textBox;
        }

        public static CheckedListBox checkedListBox(Label header, Button btnNext, int spacing, int uiHeight, int uiWidth, string[] listInput)
        {
            // Crear CheckedListBox
            CheckedListBox chListBox = new CheckedListBox
            {
                Location = new Point(spacing, header.Bottom + spacing), // Posición
                Width = uiWidth - (spacing * 2), // Ancho
                Height = (btnNext.Top - spacing) - (header.Bottom + spacing), // Altura
                CheckOnClick = true // Activar con un click
            };

            // Agregar elementos al CheckedListBox
            foreach (var item in listInput)
            {
                chListBox.Items.Add(item);
            }

            return chListBox;
        }

        public static ComboBox comboBox(Label header, int spacing, int uiHeight, int uiWidth, object[] listInput)
        {
            // Construir el ComboBox
            ComboBox cBox = new ComboBox
            {
                Location = new Point(spacing, header.Bottom + spacing), // Posición
                Width = uiWidth - (spacing * 2), // Ancho
                DropDownStyle = ComboBoxStyle.DropDownList // Configurar estilo
            };

            // Agregar elementos al ComboBox
            cBox.Items.AddRange(listInput);

            return cBox;
        }

        public static Button button_Next(int uiWidth, int spacing, int uiHeight)
        {
            // Crear botón "Next"
            Button btnNext = new Button
            {
                Text = "Next", // Texto del botón
                AutoSize = true, // Hacer que el tamaño del botón se adapte al texto
                TextAlign = ContentAlignment.MiddleCenter, // Centrar el texto
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right // Anclar el botón en la esquina inferior derecha
            };

            // Configurar la posición del botón
            btnNext.Location = new Point(
                uiWidth - (btnNext.PreferredSize.Width + spacing), // X: alineado a la derecha con espaciado
                uiHeight - (btnNext.PreferredSize.Height + spacing) // Y: alineado abajo con espaciado
            );

            return btnNext;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Clases
            // 
            this.ClientSize = new System.Drawing.Size(292, 212);
            this.Name = "Clases";
            this.Load += new System.EventHandler(this.Clases_Load);
            this.ResumeLayout(false);

        }

        private void Clases_Load(object sender, EventArgs e)
        {

        }
    }
}
