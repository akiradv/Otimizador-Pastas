using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace OtimizadorDePastas
{
    public class AboutForm : Form
    {
        private Label lblMemoryUsage;
        private System.Windows.Forms.Timer timer;

        public AboutForm()
        {
            this.Text = "Sobre o Otimizador de Pastas";
            this.Size = new Size(480, 400);  // Ajustando o tamanho para ficar mais proporcional ao screenshot
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = Properties.Resources.AppIcon;

            // Aplicar tema conforme configuração atual
            UpdateTheme(Settings.Current.IsDarkMode);

            // Painel principal com borda
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20),
                BackColor = Settings.Current.IsDarkMode ? DarkTheme.BackgroundColor : SystemColors.Control
            };

            // Painel superior (ícone e informações básicas)
            TableLayoutPanel topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Settings.Current.IsDarkMode ? DarkTheme.BackgroundColor : SystemColors.Control
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Reduzindo para a logo
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));

            // Painel de informações detalhadas
            TableLayoutPanel infoPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Settings.Current.IsDarkMode ? DarkTheme.BackgroundColor : SystemColors.Control
            };
            for (int i = 0; i < 7; i++)
            {
                infoPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            // Painel para o logo com borda opcional
            Panel logoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5),
                BackColor = Settings.Current.IsDarkMode ? DarkTheme.BackgroundColor : SystemColors.Control
            };

            // Criação do PictureBox para o ícone com tamanho fixo
            PictureBox pictureBox = new PictureBox
            {
                Image = Properties.Resources.AppLogo,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(80, 80),
                Location = new Point(5, 5),
                BackColor = Color.Transparent
            };
            logoPanel.Controls.Add(pictureBox);

            // Informações básicas utilizando um FlowLayoutPanel para melhor posicionamento vertical
            FlowLayoutPanel basicInfoFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Settings.Current.IsDarkMode ? DarkTheme.BackgroundColor : SystemColors.Control
            };
            
            Label lblTitle = new Label
            {
                Text = "Otimizador de Pastas",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5),
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            // Obtém a versão do assembly
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionText = $"{version.Major}.{version.Minor}.{version.Build}";

            Label lblVersion = new Label
            {
                Text = $"Versão {versionText}",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5),
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            Label lblCopyright = new Label
            {
                Text = "(c) 2025 AkiraDev. Todos os direitos reservados.",
                AutoSize = true,
                Margin = new Padding(0),
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            // Adiciona os controles ao FlowLayoutPanel
            basicInfoFlow.Controls.Add(lblTitle);
            basicInfoFlow.Controls.Add(lblVersion);
            basicInfoFlow.Controls.Add(lblCopyright);

            // Informações detalhadas com texto mais curto para evitar quebra
            Label lblDescription = new Label
            {
                Text = "O Otimizador de Pastas é uma ferramenta para limpeza de arquivos temporários e otimização do sistema.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                MaximumSize = new Size(410, 0), // Ajustado para caber melhor
                MinimumSize = new Size(410, 30),
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            DateTime creationTime;
            try
            {
                // Tenta obter do assembly
                creationTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            }
            catch
            {
                // Fallback: usar a data do próprio assembly
                creationTime = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day);
                
                // Ou use um valor fixo (recomendado)
                // creationTime = new DateTime(2025, 1, 29, 21, 45, 0);
            }

            Label lblCompiled = new Label
            {
                Text = $"Data de compilação: {creationTime:dd/MM/yyyy HH:mm}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            Label lblFramework = new Label
            {
                Text = $"Framework: .NET {Environment.Version}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            lblMemoryUsage = new Label
            {
                Text = "Uso de memória: Calculando...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };

            LinkLabel linkLabel = new LinkLabel
            {
                Text = "github.com/akiradv/OtimizadorDePastas",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                LinkColor = Settings.Current.IsDarkMode ? Color.LightSkyBlue : Color.Blue,
                ActiveLinkColor = Color.Orange,
                ForeColor = Settings.Current.IsDarkMode ? DarkTheme.ForegroundColor : SystemColors.ControlText
            };
            linkLabel.LinkClicked += (s, e) => Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/akiradv/OtimizadorDePastas",
                UseShellExecute = true
            });

            // Botão de fechar sem ícone
            Button btnClose = new Button
            {
                Text = "Fechar",
                Dock = DockStyle.Fill,
                Height = 40,
                Width = 100,
                Margin = new Padding(5),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10, 0, 10, 0)
            };

            if (Settings.Current.IsDarkMode)
            {
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.FlatAppearance.BorderSize = 1;
                btnClose.BackColor = DarkTheme.ButtonBackColor;
                btnClose.ForeColor = DarkTheme.ForegroundColor;
                
                btnClose.MouseEnter += (s, e) => {
                    btnClose.BackColor = DarkTheme.ButtonHoverColor;
                    btnClose.FlatAppearance.BorderColor = Color.White;
                };
                btnClose.MouseLeave += (s, e) => {
                    btnClose.BackColor = DarkTheme.ButtonBackColor;
                    btnClose.FlatAppearance.BorderColor = Color.Gray;
                };
            }
            else
            {
                btnClose.FlatStyle = FlatStyle.Standard;
                btnClose.UseVisualStyleBackColor = true;
            }

            btnClose.Click += (s, e) => this.Close();

            // Adiciona o ícone e informações básicas ao painel superior
            topPanel.Controls.Add(logoPanel, 0, 0);
            topPanel.Controls.Add(basicInfoFlow, 1, 0);

            // Adiciona as informações detalhadas ao painel de informações
            infoPanel.Controls.Add(new Label { Height = 5 }, 0, 0); // Espaçador menor
            infoPanel.Controls.Add(lblDescription, 0, 1);
            infoPanel.Controls.Add(lblCompiled, 0, 2);
            infoPanel.Controls.Add(lblFramework, 0, 3);
            infoPanel.Controls.Add(lblMemoryUsage, 0, 4);
            infoPanel.Controls.Add(linkLabel, 0, 5);
            infoPanel.Controls.Add(new Label { Height = 5 }, 0, 6); // Espaçador menor

            // Adiciona os painéis principal
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Controls.Add(infoPanel, 0, 1);

            // Painel para o botão centralizado na parte inferior
            TableLayoutPanel buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Settings.Current.IsDarkMode ? DarkTheme.BackgroundColor : SystemColors.Control,
                ColumnCount = 3,
                RowCount = 1
            };
            
            // Define o layout para centralizar o botão
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            
            // Adiciona o botão na coluna do meio
            buttonPanel.Controls.Add(btnClose, 1, 0);

            // Adiciona os painéis ao formulário
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);

            // Timer para atualizar informações de memória
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            this.FormClosed += (s, e) => timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            double memoryUsageMB = currentProcess.WorkingSet64 / 1024.0 / 1024.0;
            lblMemoryUsage.Text = $"Uso de memória: {memoryUsageMB:F2} MB";
        }

        private void UpdateTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                DarkTheme.ApplyTheme(this);
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
            }
        }
    }
} 