using System.Drawing;
using System.Windows.Forms;

namespace OtimizadorDePastas
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.Text = "Limpando...";
            this.Size = new Size(400, 120);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            TableLayoutPanel mainPanel = new()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 2,
                ColumnCount = 1,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 25),
                    new RowStyle(SizeType.Percent, 100)
                }
            };

            Label lblStatus = new()
            {
                Text = "Limpando arquivos temporários...",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9)
            };

            ProgressBar progressBar = new()
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Dock = DockStyle.Fill,
                Height = 23
            };

            mainPanel.Controls.Add(lblStatus, 0, 0);
            mainPanel.Controls.Add(progressBar, 0, 1);

            this.Controls.Add(mainPanel);
        }
    }
} 