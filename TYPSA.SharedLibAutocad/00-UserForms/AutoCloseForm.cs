using System;
using System.Drawing;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.UserForms
{
    public class AutoCloseMessageForm : Form
    {
        private Label messageLabel;
        private Timer closeTimer;

        public AutoCloseMessageForm(string message, int millisecondsToClose = 2000)
        {
            // Configuración base
            this.Text = "Information";
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.TopMost = true;
            this.AutoSize = false;

            // Tamaño y fuente
            Font font = new Font("Segoe UI", 10, FontStyle.Regular);
            int padding = 100;
            int maxWidth = Screen.PrimaryScreen.WorkingArea.Width / 2;

            // Medir el tamaño requerido para el texto
            Size textSize;
            using (Graphics g = CreateGraphics())
            {
                textSize = g.MeasureString(message, font, maxWidth - padding).ToSize();
            }

            // Altura final con margen extra
            int finalHeight = textSize.Height + padding;
            int finalWidth = maxWidth;

            // Aplicar tamaño y centrado
            this.Size = new Size(finalWidth, finalHeight);
            this.Location = Clases.centrar_Formulario(Screen.PrimaryScreen.WorkingArea, finalWidth, finalHeight);

            // Label del mensaje
            messageLabel = new Label()
            {
                Text = message,
                Font = font,
                Size = textSize,
                Location = new Point(padding / 2, padding / 2),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft,
                MaximumSize = new Size(finalWidth - padding, 0),
                Dock = DockStyle.Fill
            };
            this.Controls.Add(messageLabel);

            // Timer para cerrar automáticamente
            closeTimer = new Timer();
            closeTimer.Interval = millisecondsToClose;
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                this.Close();
            };
            closeTimer.Start();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AutoCloseMessageForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "AutoCloseMessageForm";
            this.Load += new System.EventHandler(this.AutoCloseMessageForm_Load);
            this.ResumeLayout(false);

        }

        private void AutoCloseMessageForm_Load(object sender, EventArgs e)
        {

        }
    }
}
