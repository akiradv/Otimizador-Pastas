using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OtimizadorDePastas
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon = new();
        private ContextMenuStrip contextMenu = new();
        private static Mutex? mutex;
        private CancellationTokenSource cancellationTokenSource = new();
        private List<HistoricoLimpeza> historico = new();
        private ConfigForm? configForm;
        private System.Windows.Forms.Timer timerLimpeza = new();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public Form1()
        {
            InitializeComponent();
            this.Text = "Otimizador de Pastas";
            this.StartPosition = FormStartPosition.CenterScreen;

            cancellationTokenSource = new CancellationTokenSource();

            this.Size = new Size(400, 500);
            this.MinimumSize = new Size(350, 400);
            
            TableLayoutPanel mainPanel = new()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 4,
                ColumnCount = 1,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 120), // Logo
                    new RowStyle(SizeType.Absolute, 60),  // Título
                    new RowStyle(SizeType.Percent, 100),  // Botões
                    new RowStyle(SizeType.Absolute, 30)   // Status
                }
            };

            // Logo em alta qualidade
            PictureBox logoBox = new()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            try
            {
                // Carrega o ícone do aplicativo
                Icon appIcon = Properties.Resources.AppIcon;
                this.Icon = appIcon;
                notifyIcon.Icon = appIcon;

                // Carrega a logo para o PictureBox
                if (logoBox != null)
                {
                    logoBox.SizeMode = PictureBoxSizeMode.Zoom;
                    logoBox.Image = Properties.Resources.AppLogo; // Assumindo que você tem um recurso AppLogo
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar ícones: {ex.Message}");
            }

            // Título
            Label lblTitle = new()
            {
                Text = "Otimizador de Pastas",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Painel de botões
            TableLayoutPanel buttonPanel = new()
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(10),
                RowStyles = {
                    new RowStyle(SizeType.Percent, 33.33f),
                    new RowStyle(SizeType.Percent, 33.33f),
                    new RowStyle(SizeType.Percent, 33.33f)
                }
            };

            Button btnLimpar = CreateButton("Limpar Pastas Temporárias", "🧹");
            btnLimpar.Click += ClearTempItem_Click;

            Button btnConfig = CreateButton("Configurações", "⚙️");
            btnConfig.Click += (s, e) => ShowConfigForm();

            Button btnInfo = CreateButton("Informações", "ℹ️");
            btnInfo.Click += InfoItem_Click;

            buttonPanel.Controls.Add(btnLimpar, 0, 0);
            buttonPanel.Controls.Add(btnConfig, 0, 1);
            buttonPanel.Controls.Add(btnInfo, 0, 2);

            // Status bar
            Label lblStatus = new()
            {
                Text = "Pronto",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9)
            };

            mainPanel.Controls.Add(logoBox, 0, 0);
            mainPanel.Controls.Add(lblTitle, 0, 1);
            mainPanel.Controls.Add(buttonPanel, 0, 2);
            mainPanel.Controls.Add(lblStatus, 0, 3);

            this.Controls.Add(mainPanel);

            UpdateTheme(Settings.Current.IsDarkMode);

            // Configura o NotifyIcon
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.AppIcon,
                Visible = true,
                Text = "Otimizador de Pastas"
            };

            // Corrige o comportamento do duplo clique
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            InitializeContextMenu();

            // Carrega o histórico salvo
            try
            {
                historico.Clear();
                historico.AddRange(Settings.LoadHistory());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar histórico: {ex.Message}");
            }

            timerLimpeza.Tick += TimerLimpeza_Tick;
            AtualizarLimpezaAutomatica();
        }

        private Button CreateButton(string text, string emoji)
        {
            return new Button
            {
                Text = $"{emoji} {text}",
                Dock = DockStyle.Fill,
                Height = 50,
                Margin = new Padding(5),
                Font = new Font("Segoe UI", 12),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = true
            };
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            this.Show();
            this.BringToFront();
        }

        private void ClearTempItem_Click(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn) btn.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var progress = new ProgressForm();
                progress.Show();

                var registroLimpeza = new HistoricoLimpeza { DataHora = DateTime.Now };
                long espacoTotal = 0;
                int arquivosTotal = 0;

                Task.Run(() =>
                {
                    try
                    {
                        var userTempPath = Path.GetTempPath();
                        Console.WriteLine($"Limpando pasta: {userTempPath}");
                        (int arquivosTemp1, long espacoTemp1) = LimparPasta(userTempPath);
                        espacoTotal += espacoTemp1;
                        arquivosTotal += arquivosTemp1;

                        var windowsTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
                        Console.WriteLine($"Limpando pasta: {windowsTempPath}");
                        (int arquivosTemp2, long espacoTemp2) = LimparPasta(windowsTempPath);
                        espacoTotal += espacoTemp2;
                        arquivosTotal += arquivosTemp2;

                        var prefetchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
                        Console.WriteLine($"Limpando pasta: {prefetchPath}");
                        (int arquivosPrefetch, long espacoPrefetch) = LimparPasta(prefetchPath);
                        espacoTotal += espacoPrefetch;
                        arquivosTotal += arquivosPrefetch;

                        this.Invoke(() =>
                        {
                            registroLimpeza.ArquivosLimpados = arquivosTotal;
                            registroLimpeza.EspacoLiberado = espacoTotal;
                            historico.Add(registroLimpeza);
                            
                            SalvarHistorico();

                            if (configForm != null && !configForm.IsDisposed && configForm.Visible)
                            {
                                configForm.UpdateHistorico(historico);
                            }

                            MessageBox.Show(
                                $"Limpeza concluída com sucesso!\n" +
                                $"Arquivos removidos: {registroLimpeza.ArquivosLimpados}\n" +
                                $"Espaço liberado: {registroLimpeza.EspacoLiberado / 1024.0 / 1024.0:F2} MB",
                                "Sucesso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            if (sender is Button button) button.Enabled = true;
                            Cursor = Cursors.Default;

                            progress.Close();
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(() =>
                        {
                            progress.Close();
                            MessageBox.Show($"Erro durante a limpeza: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (sender is Button button) button.Enabled = true;
                            Cursor = Cursors.Default;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar limpeza: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (sender is Button btn) btn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void BtnLimpar_Click(object? sender, EventArgs e)
        {
            ClearTempItem_Click(sender, e);
        }

        private void ExitItem_Click(object? sender, EventArgs e)
        {
            try
            {
                // Salva o histórico antes de fechar
                SalvarHistorico();
                
                // Libera recursos
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                }

                // Libera o mutex apenas se ele não foi descartado
                if (mutex != null && !mutex.WaitOne(0)) // Verifica se o mutex está disponível
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }

                // Fecha o aplicativo
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao fechar o aplicativo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InfoItem_Click(object? sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            string formattedVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            string dotNetVersion = Environment.Version.ToString();

            MessageBox.Show($"Versão da Aplicação: {formattedVersion}\nVersão do .NET: {dotNetVersion}", "Informações", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private (int arquivos, long espaco) LimparPasta(string pasta)
        {
            int arquivosLimpados = 0;
            long espacoLiberado = 0;

            try
            {
                if (!Directory.Exists(pasta))
                {
                    Console.WriteLine($"Pasta não existe: {pasta}");
                    return (0, 0);
                }

                var arquivosParaExcluir = new List<FileInfo>();
                foreach (var arquivo in Directory.GetFiles(pasta, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var fileInfo = new FileInfo(arquivo);
                        if (!fileInfo.Exists) continue;

                        if (!IsFileLocked(fileInfo))
                        {
                            arquivosParaExcluir.Add(fileInfo);
                            espacoLiberado += fileInfo.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao acessar arquivo {arquivo}: {ex.Message}");
                        continue;
                    }
                }

                foreach (var fileInfo in arquivosParaExcluir)
                {
                    try
                    {
                        fileInfo.Delete();
                        arquivosLimpados++;
                        Console.WriteLine($"Arquivo excluído: {fileInfo.FullName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir arquivo {fileInfo.FullName}: {ex.Message}");
                        continue;
                    }
                }

                foreach (var subPasta in Directory.GetDirectories(pasta, "*", SearchOption.AllDirectories).Reverse())
                {
                    try
                    {
                        if (Directory.GetFiles(subPasta).Length == 0 && 
                            Directory.GetDirectories(subPasta).Length == 0)
                        {
                            Directory.Delete(subPasta, false);
                            Console.WriteLine($"Pasta vazia excluída: {subPasta}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta {subPasta}: {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar pasta {pasta}: {ex.Message}");
            }

            return (arquivosLimpados, espacoLiberado);
        }

        private bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        private void HistoricoItem_Click(object sender, EventArgs e)
        {
            if (historico.Count == 0)
            {
                MessageBox.Show("Nenhuma limpeza foi realizada ainda.", "Histórico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string historicoCompleto = string.Join("\n", historico);
            MessageBox.Show(historicoCompleto, "Histórico de Limpeza", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                // Libera recursos ao fechar
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                }

                // Libera o mutex apenas se ele não foi descartado
                if (mutex != null && !mutex.WaitOne(0)) // Verifica se o mutex está disponível
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }

        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            
            try
            {
                // Configura os itens do menu sem ícones
                var clearTempItem = new ToolStripMenuItem("Limpar Arquivos Temporários");
                clearTempItem.Click += ClearTempItem_Click;

                var configItem = new ToolStripMenuItem("Configurações");
                configItem.Click += (s, e) => ShowConfigForm();

                var infoItem = new ToolStripMenuItem("Informações");
                infoItem.Click += InfoItem_Click;

                var exitItem = new ToolStripMenuItem("Sair");
                exitItem.Click += ExitItem_Click;

                var separator1 = new ToolStripSeparator();
                var separator2 = new ToolStripSeparator();

                contextMenu.Items.AddRange(new ToolStripItem[] {
                    clearTempItem,
                    separator1,
                    configItem,
                    infoItem,
                    separator2,
                    exitItem
                });

                notifyIcon.ContextMenuStrip = contextMenu;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar menu de contexto: {ex.Message}");
            }
        }

        private void ShowConfigForm()
        {
            if (configForm == null || configForm.IsDisposed)
            {
                configForm = new ConfigForm(this);
            }

            if (!configForm.Visible)
            {
                configForm.Show();
                configForm?.UpdateHistorico(historico);
            }
            else
            {
                configForm.BringToFront();
            }
        }

        public void UpdateTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                DarkTheme.ApplyTheme(this);
                DarkTheme.StyleContextMenu(contextMenu);
            }
            else
            {
                // Implementar tema claro
            }
        }

        // Método para salvar o histórico
        private void SalvarHistorico()
        {
            Settings.SaveHistory(historico);
        }

        private void CarregarHistorico()
        {
            var historicoCarregado = Settings.LoadHistory();
            historico.Clear();
            historico.AddRange(historicoCarregado);
            configForm?.UpdateHistorico(historico);
        }

        public void AtualizarLimpezaAutomatica()
        {
            if (Settings.Current.LimpezaAutomaticaAtivada)
            {
                int intervaloBase = Settings.Current.IntervaloLimpeza;
                int multiplicador = Settings.Current.UnidadeTempo switch
                {
                    0 => 60000,        // Minutos para milissegundos
                    1 => 3600000,      // Horas para milissegundos
                    2 => 86400000,     // Dias para milissegundos
                    _ => 60000
                };

                timerLimpeza.Interval = intervaloBase * multiplicador;
                timerLimpeza.Start();
                Console.WriteLine($"Timer iniciado com intervalo de {intervaloBase} {Settings.Current.UnidadeTempo switch { 0 => "minutos", 1 => "horas", 2 => "dias", _ => "minutos" }}");
            }
            else
            {
                timerLimpeza.Stop();
                Console.WriteLine("Timer parado");
            }
        }

        private void TimerLimpeza_Tick(object? sender, EventArgs e)
        {
            try
            {
                var registroLimpeza = new HistoricoLimpeza { DataHora = DateTime.Now };
                long espacoTotal = 0;
                int arquivosTotal = 0;

                Task.Run(() =>
                {
                    try
                    {
                        var userTempPath = Path.GetTempPath();
                        Console.WriteLine($"Limpando pasta: {userTempPath}");
                        (int arquivosTemp1, long espacoTemp1) = LimparPasta(userTempPath);
                        espacoTotal += espacoTemp1;
                        arquivosTotal += arquivosTemp1;

                        var windowsTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
                        Console.WriteLine($"Limpando pasta: {windowsTempPath}");
                        (int arquivosTemp2, long espacoTemp2) = LimparPasta(windowsTempPath);
                        espacoTotal += espacoTemp2;
                        arquivosTotal += arquivosTemp2;

                        var prefetchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
                        Console.WriteLine($"Limpando pasta: {prefetchPath}");
                        (int arquivosPrefetch, long espacoPrefetch) = LimparPasta(prefetchPath);
                        espacoTotal += espacoPrefetch;
                        arquivosTotal += arquivosPrefetch;

                        this.Invoke(() =>
                        {
                            // Só adiciona ao histórico e mostra notificação se algo foi limpo
                            if (arquivosTotal > 0 || espacoTotal > 0)
                            {
                                registroLimpeza.ArquivosLimpados = arquivosTotal;
                                registroLimpeza.EspacoLiberado = espacoTotal;
                                historico.Add(registroLimpeza);
                                
                                SalvarHistorico();

                                if (configForm != null && !configForm.IsDisposed && configForm.Visible)
                                {
                                    configForm.UpdateHistorico(historico);
                                }

                                // Mostra notificação apenas se houver limpeza e o form estiver minimizado/oculto
                                if (this.WindowState == FormWindowState.Minimized || !this.Visible)
                                {
                                    notifyIcon.ShowBalloonTip(
                                        5000,
                                        "Limpeza Automática Concluída",
                                        $"Arquivos removidos: {registroLimpeza.ArquivosLimpados}\n" +
                                        $"Espaço liberado: {registroLimpeza.EspacoLiberado / 1024.0 / 1024.0:F2} MB",
                                        ToolTipIcon.Info
                                    );
                                }
                            }
                            else
                            {
                                Console.WriteLine("Nenhum arquivo para limpar encontrado.");
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(() =>
                        {
                            notifyIcon.ShowBalloonTip(
                                5000,
                                "Erro na Limpeza Automática",
                                $"Ocorreu um erro: {ex.Message}",
                                ToolTipIcon.Error
                            );
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no timer de limpeza: {ex.Message}");
            }
        }
    }
}