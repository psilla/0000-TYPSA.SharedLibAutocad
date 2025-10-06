namespace TYPSA.SharedLib.Autocad.UserForms
{
    partial class ProjectCodeDialogTask
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectCodeDialogTask));
            this.lblProjectCode = new System.Windows.Forms.Label();
            this.txtProjectCode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblProjectCode
            // 
            this.lblProjectCode.AutoSize = true;
            this.lblProjectCode.Location = new System.Drawing.Point(277, 45);
            this.lblProjectCode.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblProjectCode.Name = "lblProjectCode";
            this.lblProjectCode.Size = new System.Drawing.Size(291, 32);
            this.lblProjectCode.TabIndex = 0;
            this.lblProjectCode.Text = "Enter the project code";
            this.lblProjectCode.Click += new System.EventHandler(this.lblProjectCode_Click);
            // 
            // txtProjectCode
            // 
            this.txtProjectCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProjectCode.Location = new System.Drawing.Point(61, 112);
            this.txtProjectCode.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtProjectCode.Name = "txtProjectCode";
            this.txtProjectCode.Size = new System.Drawing.Size(716, 38);
            this.txtProjectCode.TabIndex = 1;
            this.txtProjectCode.TextChanged += new System.EventHandler(this.txtProjectCode_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(341, 188);
            this.button1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 55);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ProjectCodeDialogTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 284);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtProjectCode);
            this.Controls.Add(this.lblProjectCode);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "ProjectCodeDialogTask";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Code Dialog";
            this.Load += new System.EventHandler(this.ProjectCodeDialogTask_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblProjectCode;
        public System.Windows.Forms.TextBox txtProjectCode;
        public System.Windows.Forms.Button button1;
    }
}