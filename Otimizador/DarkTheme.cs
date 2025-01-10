using System.Drawing.Drawing2D;
using System.Windows.Forms;

public static class DarkTheme
{
    // Cores mais agradáveis inspiradas no Visual Studio Dark
    public static Color BackgroundColor = Color.FromArgb(45, 45, 48);
    public static Color ForegroundColor = Color.FromArgb(241, 241, 241);
    public static Color MenuBackColor = Color.FromArgb(28, 28, 28);
    public static Color MenuSelectedColor = Color.FromArgb(62, 62, 66);
    public static Color MenuSeparatorColor = Color.FromArgb(61, 61, 61);
    public static Color ButtonBackColor = Color.FromArgb(51, 51, 55);
    public static Color ButtonHoverColor = Color.FromArgb(67, 67, 70);
    public static Color TabBackColor = Color.FromArgb(37, 37, 38);
    public static Color GridBackColor = Color.FromArgb(37, 37, 38);
    public static Color GridHeaderColor = Color.FromArgb(51, 51, 55);

    public static void StyleDataGridView(DataGridView dgv, bool darkMode)
    {
        if (darkMode)
        {
            dgv.BackgroundColor = GridBackColor;
            dgv.ForeColor = ForegroundColor;
            dgv.GridColor = MenuSeparatorColor;
            dgv.DefaultCellStyle.BackColor = GridBackColor;
            dgv.DefaultCellStyle.ForeColor = ForegroundColor;
            dgv.DefaultCellStyle.SelectionBackColor = MenuSelectedColor;
            dgv.DefaultCellStyle.SelectionForeColor = ForegroundColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ForegroundColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
        }
        else
        {
            dgv.BackgroundColor = SystemColors.Window;
            dgv.ForeColor = SystemColors.ControlText;
            dgv.GridColor = SystemColors.Control;
            dgv.DefaultCellStyle.BackColor = SystemColors.Window;
            dgv.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            dgv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            dgv.EnableHeadersVisualStyles = true;
            dgv.BorderStyle = BorderStyle.Fixed3D;
        }
    }

    public static void StyleTabControl(TabControl tab, bool darkMode)
    {
        if (darkMode)
        {
            tab.BackColor = TabBackColor;
            tab.ForeColor = ForegroundColor;
        }
        else
        {
            tab.BackColor = SystemColors.Control;
            tab.ForeColor = SystemColors.ControlText;
        }
    }

    public static void ApplyTheme(Form form)
    {
        form.BackColor = BackgroundColor;
        form.ForeColor = ForegroundColor;

        foreach (Control control in form.Controls)
        {
            if (control is Button button)
            {
                StyleButton(button);
            }
        }
    }

    public static void StyleContextMenu(ContextMenuStrip menu)
    {
        menu.BackColor = MenuBackColor;
        menu.ForeColor = ForegroundColor;
        menu.RenderMode = ToolStripRenderMode.Professional;
        menu.Renderer = new DarkMenuRenderer();

        foreach (ToolStripItem item in menu.Items)
        {
            item.BackColor = MenuBackColor;
            item.ForeColor = ForegroundColor;
        }
    }

    private static void StyleButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.BackColor = ButtonBackColor;
        button.ForeColor = ForegroundColor;
        
        button.MouseEnter += (s, e) => button.BackColor = ButtonHoverColor;
        button.MouseLeave += (s, e) => button.BackColor = ButtonBackColor;
    }
}

public class DarkMenuRenderer : ToolStripProfessionalRenderer
{
    public DarkMenuRenderer() : base(new DarkColors()) { }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (!e.Item.Selected)
            return;

        using var brush = new SolidBrush(DarkTheme.MenuSelectedColor);
        e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
    }
}

public class DarkColors : ProfessionalColorTable
{
    public override Color MenuItemSelected => DarkTheme.MenuSelectedColor;
    public override Color MenuItemBorder => Color.Transparent;
    public override Color MenuBorder => DarkTheme.MenuBackColor;
    public override Color MenuItemSelectedGradientBegin => DarkTheme.MenuSelectedColor;
    public override Color MenuItemSelectedGradientEnd => DarkTheme.MenuSelectedColor;
} 