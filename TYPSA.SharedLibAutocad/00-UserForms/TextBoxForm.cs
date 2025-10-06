using System;
using System.Windows.Forms;
using System.Drawing;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public class TextBoxForm : Form
    {
        private Label header;
        private Label label;
        private System.Windows.Forms.TextBox textBox;
        private Button btnNext;
        public string salida = null; // Almacena el texto ingresado

        public TextBoxForm(string mensajeSel, string defaultValue = "")
        {
            // Configuración del formulario
            this.Text = "Enter a numeric value";
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.salida = null;

            // Dimensiones y espaciado
            var screenSize = Screen.PrimaryScreen.WorkingArea;
            this.Width = screenSize.Width / 2;
            this.Height = screenSize.Height / 4;
            int spacing = 10;
            int uiWidth = this.ClientSize.Width;
            int uiHeight = this.ClientSize.Height;

            // Añadimos controles
            this.Location = Clases.centrar_Formulario(screenSize, this.Width, this.Height);

            // Header
            header = Clases.label_Header(mensajeSel, spacing);
            this.Controls.Add(header);

            // Label
            label = Clases.label_TextBox(this.ClientSize, spacing);
            this.Controls.Add(label);

            // TextBox
            textBox = Clases.textBox(uiWidth, spacing, label, this.ClientSize);
            textBox.Text = defaultValue; // 👈 asignar valor inicial si se pasa
            this.Controls.Add(textBox);

            // Botón Next
            btnNext = Clases.button_Next(uiWidth, spacing, uiHeight);
            btnNext.Click += OnButtonClick;
            btnNext.Click += NextButtonPressed;
            this.Controls.Add(btnNext);

            // Enter activa el botón Next
            this.AcceptButton = btnNext;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            salida = textBox.Text;

            if (string.IsNullOrWhiteSpace(salida))
            {
                // Mensaje
                MessageBox.Show("" +
                    "Por favor, introduce un valor antes de continuar.",
                    "Campo vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                // No cerramos el formulario
                return;
            }
            this.Close();
        }


        private void NextButtonPressed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TextBoxForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 212);
            this.Name = "TextBoxForm";
            this.Load += new System.EventHandler(this.TextBoxForm_Load);
            this.ResumeLayout(false);

        }

        private void TextBoxForm_Load(object sender, EventArgs e)
        {

        }
    }

}



