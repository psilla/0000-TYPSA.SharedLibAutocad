using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public partial class ProjectCodeDialogTask : Form
    {
        public string ProjectCode { get; private set; }

        public ProjectCodeDialogTask()
        {
            InitializeComponent();
        }

        // Evento que se dispara cuando se hace clic en el botón "OK"
        private void button1_Click(object sender, EventArgs e)
        {
            ProjectCode = txtProjectCode.Text;

            // Validate the project code format
            if (IsValidProjectCode(ProjectCode))
            {
                MessageBox.Show($"The entered project code is: {ProjectCode}", "Project Code Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Close the window after clicking OK
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("The project code must be in the format LLNNNN (e.g., AB1234). Please try again.", "Invalid Project Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Keep the form open
                txtProjectCode.Focus();
                txtProjectCode.SelectAll();
            }
        }

        private bool IsValidProjectCode(string projectCode)
        {
            // Regex for format: Letter-Letter-Number-Number-Number-Number
            string pattern = @"^[A-Za-z]{2}\d{4}$";
            return Regex.IsMatch(projectCode, pattern);
        }

        private void ProjectCodeDialogTask_Load(object sender, EventArgs e)
        {

        }

        private void txtProjectCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void ProjectCodeDialogTask_Load_1(object sender, EventArgs e)
        {

        }

        private void lblProjectCode_Click(object sender, EventArgs e)
        {

        }
    }
}
