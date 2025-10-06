using System;
using System.Drawing;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public partial class ProgressBarControl : Form
    {
        public ProgressBarControl()
        {
            InitializeComponent();
        }

        // Agregar una propiedad para controlar el valor de la barra de progreso
        public int ProgressValue
        {
            get { return progressBar1.Value; }
            set { progressBar1.Value = value; }
        }

        private void ProgressBarControl_Load(object sender, EventArgs e)
        {
            // Obtener las dimensiones de la pantalla
            var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // Calcular la nueva posición del formulario
            var formWidth = this.Width;
            var formHeight = this.Height;
            var newX = screenWidth / 2 + screenWidth / 4 - formWidth / 2;
            var newY = screenHeight / 4 - formHeight / 2;

            // Establecer la nueva posición del formulario
            this.Location = new Point(newX, newY);

            // Asegurar que el formulario esté siempre encima
            this.TopMost = true;
        }
    }
}
