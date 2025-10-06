using System;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public partial class PathEntry : Form
    {
        private string projectCode;

        // Propiedad para devolver la lista de archivos seleccionados
        public string[] SelectedFiles { get; private set; }
        // Propiedad para devolver la ruta ingresada manualmente
        public string UserEnteredPath { get; private set; }

        public PathEntry(string projectCode)
        {
            InitializeComponent();
            this.projectCode = projectCode;
            SelectedFiles = Array.Empty<string>(); // Inicializar vacío
            UserEnteredPath = string.Empty; // Inicializar vacío
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Obtener la ruta ingresada por el usuario
            string userPath = textBox1.Text.Trim();
            UserEnteredPath = userPath;

            // Crear un cuadro de diálogo para seleccionar archivos
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = userPath,
                Title = "Select Files to Analyze",
                Filter = "Civil Files (*.dwg)|*.dwg|All Files (*.*)|*.*",
                Multiselect = true // Permite seleccionar múltiples archivos
            };

            // Mostrar el cuadro de diálogo y almacenar las rutas seleccionadas
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedFiles = openFileDialog.FileNames; // Rutas seleccionadas
                this.DialogResult = DialogResult.OK; // Indicar que el formulario se cerró exitosamente
            }
            else if (!string.IsNullOrEmpty(userPath))
            {
                // Si no se seleccionaron archivos, pero el usuario ingresó una ruta
                SelectedFiles = new string[] { userPath }; // Devuelve solo la ruta manual
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                // Si no se seleccionaron archivos ni se ingresó una ruta, mostrar un mensaje
                MessageBox.Show("Please select files or enter a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Close(); // Cerrar el formulario
        }

        private void PathEntry_Load(object sender, EventArgs e)
        {

        }
    }
}

