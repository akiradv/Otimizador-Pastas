using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OtimizadorDePastas
{
    public partial class ConfigForm : Form
    {
        private readonly Form1 mainForm;
        private DataGridView dgvHistorico = null!;
        private TabControl tabControl = null!;
        private CheckBox chkDarkMode = null!;
        private NumericUpDown nudIntervalo = null!;
        private ComboBox cmbUnidadeTempo = null!;
        private CheckBox chkLimpezaAutomatica = null!;

        public ConfigForm(Form1 mainForm)
        {
            this.mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            InitializeComponent();
            this.Text = "Configurações";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponents();
            
            // Carrega o tema das configurações
            chkDarkMode.Checked = Settings.Current.IsDarkMode;
            ApplyTheme(Settings.Current.IsDarkMode);
        }

        private void InitializeComponents()
        {
            this.MinimumSize = new Size(600, 400);
            
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(12, 4),
                Font = new Font("Segoe UI", 10)
            };

            // Tab Histórico
            var tabHistorico = new TabPage("📋 Histórico");
            dgvHistorico = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9),
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                BackgroundColor = SystemColors.Window,
                GridColor = Color.FromArgb(230, 230, 230)
            };

            dgvHistorico.Columns.Add("DataHora", "Data/Hora");
            dgvHistorico.Columns.Add("Arquivos", "Arquivos Removidos");
            dgvHistorico.Columns.Add("Espaco", "Espaço Liberado (MB)");

            // Estilo do cabeçalho
            dgvHistorico.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvHistorico.EnableHeadersVisualStyles = false;

            tabHistorico.Controls.Add(dgvHistorico);

            // Tab Aparência
            var tabAparencia = new TabPage("🎨 Aparência");
            var pnlAparencia = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 3,
                ColumnCount = 1,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 40),
                    new RowStyle(SizeType.Absolute, 40),
                    new RowStyle(SizeType.Percent, 100)
                }
            };

            chkDarkMode = new CheckBox
            {
                Text = "Modo Escuro",
                Checked = Settings.Current.IsDarkMode,
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            pnlAparencia.Controls.Add(chkDarkMode, 0, 0);
            tabAparencia.Controls.Add(pnlAparencia);

            // Tab Limpeza Automática
            var tabLimpezaAuto = new TabPage("⏰ Limpeza Automática");
            var pnlLimpezaAuto = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 4,
                ColumnCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 40),
                    new RowStyle(SizeType.Absolute, 40),
                    new RowStyle(SizeType.Absolute, 40),
                    new RowStyle(SizeType.Percent, 100)
                },
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                }
            };

            chkLimpezaAutomatica = new CheckBox
            {
                Text = "Ativar limpeza automática",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Checked = Settings.Current.LimpezaAutomaticaAtivada
            };

            var lblIntervalo = new Label
            {
                Text = "Intervalo de limpeza:",
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            nudIntervalo = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 999,
                Value = Settings.Current.IntervaloLimpeza,
                Font = new Font("Segoe UI", 10),
                Width = 80
            };

            cmbUnidadeTempo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Items = { "Minutos", "Horas", "Dias" },
                SelectedIndex = Settings.Current.UnidadeTempo
            };

            var pnlIntervalo = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            pnlIntervalo.Controls.AddRange(new Control[] { nudIntervalo, cmbUnidadeTempo });

            // Adiciona controles ao painel
            pnlLimpezaAuto.Controls.Add(chkLimpezaAutomatica, 0, 0);
            pnlLimpezaAuto.Controls.Add(lblIntervalo, 0, 1);
            pnlLimpezaAuto.Controls.Add(pnlIntervalo, 1, 1);

            tabLimpezaAuto.Controls.Add(pnlLimpezaAuto);

            // Adiciona as tabs
            tabControl.TabPages.Add(tabHistorico);
            tabControl.TabPages.Add(tabAparencia);
            tabControl.TabPages.Add(tabLimpezaAuto);

            this.Controls.Add(tabControl);

            // Eventos
            chkLimpezaAutomatica.CheckedChanged += (s, e) => {
                Settings.Current.LimpezaAutomaticaAtivada = chkLimpezaAutomatica.Checked;
                Settings.Save();
                mainForm.AtualizarLimpezaAutomatica();
            };

            nudIntervalo.ValueChanged += (s, e) => {
                Settings.Current.IntervaloLimpeza = (int)nudIntervalo.Value;
                Settings.Save();
                mainForm.AtualizarLimpezaAutomatica();
            };

            cmbUnidadeTempo.SelectedIndexChanged += (s, e) => {
                Settings.Current.UnidadeTempo = cmbUnidadeTempo.SelectedIndex;
                Settings.Save();
                mainForm.AtualizarLimpezaAutomatica();
            };

            chkDarkMode.CheckedChanged += (s, e) => {
                Settings.Current.IsDarkMode = chkDarkMode.Checked;
                Settings.Save();
                ApplyTheme(chkDarkMode.Checked);
                mainForm.UpdateTheme(chkDarkMode.Checked);
            };
        }

        public void UpdateHistorico(List<HistoricoLimpeza> historico)
        {
            if (dgvHistorico.InvokeRequired)
            {
                dgvHistorico.Invoke(new Action(() => UpdateHistorico(historico)));
                return;
            }

            try
            {
                // Garante que as colunas existam
                if (dgvHistorico.Columns.Count == 0)
                {
                    dgvHistorico.Columns.Add("DataHora", "Data/Hora");
                    dgvHistorico.Columns.Add("Arquivos", "Arquivos Removidos");
                    dgvHistorico.Columns.Add("Espaco", "Espaço Liberado (MB)");
                }

                dgvHistorico.Rows.Clear();
                foreach (var item in historico)
                {
                    dgvHistorico.Rows.Add(
                        item.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                        item.ArquivosLimpados.ToString(),
                        (item.EspacoLiberado / 1024.0 / 1024.0).ToString("F2")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar histórico: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyTheme(bool darkMode)
        {
            if (darkMode)
            {
                this.BackColor = DarkTheme.BackgroundColor;
                this.ForeColor = DarkTheme.ForegroundColor;
                
                DarkTheme.StyleDataGridView(dgvHistorico, true);
                DarkTheme.StyleTabControl(tabControl, true);

                foreach (Control control in GetAllControls(this))
                {
                    switch (control)
                    {
                        case CheckBox checkBox:
                            checkBox.BackColor = DarkTheme.BackgroundColor;
                            checkBox.ForeColor = DarkTheme.ForegroundColor;
                            break;
                        case Label label:
                            label.BackColor = DarkTheme.BackgroundColor;
                            label.ForeColor = DarkTheme.ForegroundColor;
                            break;
                        case NumericUpDown nud:
                            nud.BackColor = DarkTheme.ButtonBackColor;
                            nud.ForeColor = DarkTheme.ForegroundColor;
                            break;
                        case ComboBox combo:
                            combo.BackColor = DarkTheme.ButtonBackColor;
                            combo.ForeColor = DarkTheme.ForegroundColor;
                            break;
                    }
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
                
                DarkTheme.StyleDataGridView(dgvHistorico, false);
                DarkTheme.StyleTabControl(tabControl, false);

                foreach (Control control in this.Controls)
                {
                    if (control is CheckBox checkBox)
                    {
                        checkBox.BackColor = SystemColors.Control;
                        checkBox.ForeColor = SystemColors.ControlText;
                    }
                }
            }
        }

        private IEnumerable<Control> GetAllControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls(ctrl)).Concat(controls);
        }
    }
} 