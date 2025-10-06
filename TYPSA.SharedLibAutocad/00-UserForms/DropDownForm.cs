using System;
using System.Windows.Forms;
using System.Drawing;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public class DropDownForm : Form
    {
        private Label header;
        private Button btnNext;
        private System.Windows.Forms.ComboBox comboBox;
        public object salida = null; // Almacena la salida seleccionada

        public DropDownForm(string mensajeSel, object[] listInput, bool? valorPorDefecto = null)
        {
            // Configuración del formulario
            this.Text = "Selection Form";
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.salida = null;

            // Dimensiones y espaciado
            Rectangle screenSize = Screen.PrimaryScreen.WorkingArea;
            this.Width = screenSize.Width / 2;
            this.Height = screenSize.Height / 5;
            int spacing = 10;
            int uiWidth = this.ClientSize.Width;
            int uiHeight = this.ClientSize.Height;

            // Añadimos controles
            this.Location = Clases.centrar_Formulario(screenSize, this.Width, this.Height);

            // Header
            header = Clases.label_Header(mensajeSel, spacing);
            this.Controls.Add(header);

            // Botón Next
            btnNext = Clases.button_Next(uiWidth, spacing, uiHeight);
            btnNext.Click += NextButtonPressed;
            this.Controls.Add(btnNext);

            // Enter activa el botón Next
            this.AcceptButton = btnNext;

            // ComboBox
            comboBox = Clases.comboBox(header, spacing, uiHeight, uiWidth, listInput);
            this.Controls.Add(comboBox);

            // 👉 Establecer valor por defecto si existe
            if (valorPorDefecto.HasValue)
            {
                comboBox.SelectedItem = valorPorDefecto.Value;
            }

            // Cierre Form
            this.FormClosing += OnFormClosing;

        }

        // Eventos internos
        private void NextButtonPressed(object sender, EventArgs e)
        {
            if (comboBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select an option before continuing.",
                    "Selection Required"
                );
                return;
            }

            salida = comboBox.SelectedItem;
            this.Close();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Si no se seleccionó nada, y el cierre es por el usuario (no por código)
            if (salida == null && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "No option was selected. Do you want to cancel the process?",
                    "Confirmation"
                );

                if (result == DialogResult.No)
                {
                    // Cancela el cierre
                    e.Cancel = true;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DropDownForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "DropDownForm";
            this.Load += new System.EventHandler(this.DropDownForm_Load);
            this.ResumeLayout(false);

        }

        private void DropDownForm_Load(object sender, EventArgs e)
        {

        }
    }

}







