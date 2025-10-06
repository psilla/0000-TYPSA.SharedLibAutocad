using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public class CheckListBoxForm : Form
    {
        private Label header;
        private CheckedListBox chListBox;
        private Button btnNext;

        // Almacena el texto ingresado
        public List<string> salida { get; private set; }

        public CheckListBoxForm(string mensajeSel, List<string> listInput)
        {
            // Configuración del formulario
            this.Text = "Selection Form";
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.salida = null;

            // Dimensiones y espaciado
            var screenSize = Screen.PrimaryScreen.WorkingArea;
            this.Width = screenSize.Width / 2;
            this.Height = screenSize.Height / 2;
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
            btnNext.Click += OnButtonClick;
            this.Controls.Add(btnNext);

            // CheckListBox
            chListBox =
                Clases.checkedListBox(header, btnNext, spacing, uiHeight, uiWidth, listInput.ToArray());
            chListBox.CheckOnClick = true;
            this.Controls.Add(chListBox);

            // Enter activa el botón Next
            this.AcceptButton = btnNext;

            // Cierre Form
            this.FormClosing += OnFormClosing;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            // Crear una lista con los elementos seleccionados
            salida = chListBox.CheckedItems.Cast<object>().Select(item => item.ToString()).ToList();
            // Validamos
            if (salida.Count == 0)
            {
                // Mensaje
                MessageBox.Show(
                    "Please select at least one item.",
                    "Warning"
                );
                // Finalizamos
                return;
            }

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

        // Atajos de teclado: Ctrl+A (seleccionar todo), Ctrl+D (deseleccionar todo)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                for (int i = 0; i < chListBox.Items.Count; i++)
                {
                    chListBox.SetItemChecked(i, true);
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.D))
            {
                for (int i = 0; i < chListBox.Items.Count; i++)
                {
                    chListBox.SetItemChecked(i, false);
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CheckListBoxForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "CheckListBoxForm";
            this.Load += new System.EventHandler(this.CheckListBoxForm_Load);
            this.ResumeLayout(false);

        }

        private void CheckListBoxForm_Load(object sender, EventArgs e)
        {

        }
    }






}


