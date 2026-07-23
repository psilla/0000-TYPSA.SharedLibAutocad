using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TYPSA.SharedLib.UserForms
{
    public partial class SelectFilesDialog : Form
    {
        private CheckedListBox checkedListBox1;
        private Button buttonOk;
        private Label label1;

        public List<string> SelectedFiles { get; private set; }

        public SelectFilesDialog(List<string> fileNames)
        {
            InitializeComponent();

            // Cargar archivos en la lista
            foreach (var file in fileNames)
            {
                checkedListBox1.Items.Add(file, false);
            }
        }

        private void InitializeComponent()
        {
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 40);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(360, 144);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(145, 205);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(90, 30);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(445, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select the files you want to delete:";
            // 
            // SelectFilesDialog
            // 
            this.ClientSize = new System.Drawing.Size(384, 251);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectFilesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Files";
            this.Load += new System.EventHandler(this.SelectFilesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            // Obtener archivos seleccionados
            SelectedFiles = checkedListBox1.CheckedItems.Cast<string>().ToList();

            if (SelectedFiles.Count == 0)
            {
                MessageBox.Show("Please select at least one file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SelectFilesDialog_Load(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
