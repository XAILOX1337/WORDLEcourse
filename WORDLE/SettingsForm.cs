using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WORDLE;

/// <summary>
/// Форма настроек игры Wordle
/// </summary>
public class SettingsForm : Form
{
    private int size_x = 900;
    private int size_y = 700;
    
    // Dark theme colors (default)
    private Color darkBackgroundColor = Color.FromArgb(18, 18, 19);
    private Color darkPrimaryColor = Color.FromArgb(83, 141, 78);
    private Color darkSecondaryColor = Color.FromArgb(129, 131, 132);
    private Color darkAccentColor = Color.FromArgb(83, 141, 78);
    
    // Light theme colors
    private Color lightBackgroundColor = Color.FromArgb(227, 227, 225);
    private Color lightPrimaryColor = Color.FromArgb(83, 141, 78);
    private Color lightSecondaryColor = Color.FromArgb(180, 180, 180);
    private Color lightAccentColor = Color.FromArgb(83, 141, 78);
    
    private Theme currentTheme = Theme.Dark;
    private Label titleLabel;
    private Button themeToggleButton;
    private Button backButton;

    public SettingsForm(Theme initialTheme)
    {
        this.currentTheme = initialTheme;
        InitializeSettingsForm();
    }

    private void InitializeSettingsForm()
    {
        // Настройка формы
        this.Size = new Size(size_x, size_y);
        this.Text = "WORDLE - Настройки";
        this.BackColor = backgroundColor;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        // Создаем заголовок SETTINGS большим шрифтом
        titleLabel = new Label
        {
            Text = "SETTINGS",
            Font = new Font("Calibri", 72, FontStyle.Bold),
            ForeColor = accentColor,
            BackColor = Color.Transparent,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter
        };
        titleLabel.Location = new Point(
            (size_x - titleLabel.PreferredWidth) / 2,
            size_y / 8
        );

        // Создаем кнопку переключения темы
        themeToggleButton = CreateThemeButton();
        themeToggleButton.Location = new Point(
            (size_x - themeToggleButton.Width) / 2,
            size_y / 3
        );

        // Создаем кнопку "Назад"
        backButton = CreateMenuButton("Назад", secondaryColor);
        backButton.Location = new Point(
            (size_x - backButton.Width) / 2,
            size_y / 3 + 140
        );
        backButton.Click += BackButton_Click;

        // Добавляем все элементы на форму
        this.Controls.Add(titleLabel);
        this.Controls.Add(themeToggleButton);
        this.Controls.Add(backButton);
        
        UpdateThemeDisplay();
    }

    /// <summary>
    /// Создает кнопку переключения темы
    /// </summary>
    private Button CreateThemeButton()
    {
        Button button = new Button
        {
            Text = "Theme: Dark",
            Font = new Font("Calibri", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = primaryColor,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Width = 250,
            Height = 50,
            TextAlign = ContentAlignment.MiddleCenter
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(primaryColor, 0.15f);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(primaryColor, 0.1f);

        MakeButtonRounded(button);

        button.Click += ThemeToggleButton_Click;

        return button;
    }

    /// <summary>
    /// Создает кнопку меню с заданными параметрами и закругленными углами
    /// </summary>
    private Button CreateMenuButton(string text, Color backColor)
    {
        Button button = new Button
        {
            Text = text,
            Font = new Font("Calibri", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = backColor,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Width = 250,
            Height = 50,
            TextAlign = ContentAlignment.MiddleCenter
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.15f);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.1f);

        MakeButtonRounded(button);

        return button;
    }

    /// <summary>
    /// Делает углы кнопки закругленными
    /// </summary>
    private void MakeButtonRounded(Button button)
    {
        int radius = 50;
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
        path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
        path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
        path.CloseAllFigures();
        button.Region = new Region(path);
    }

    private void ThemeToggleButton_Click(object? sender, EventArgs e)
    {
        // Переключаем тему
        currentTheme = currentTheme == Theme.Dark ? Theme.Light : Theme.Dark;
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        if (currentTheme == Theme.Dark)
        {
            this.BackColor = darkBackgroundColor;
            titleLabel.ForeColor = darkAccentColor;
            themeToggleButton.BackColor = darkPrimaryColor;
            themeToggleButton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(darkPrimaryColor, 0.15f);
            themeToggleButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(darkPrimaryColor, 0.1f);
            backButton.BackColor = darkSecondaryColor;
            backButton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(darkSecondaryColor, 0.15f);
            backButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(darkSecondaryColor, 0.1f);
        }
        else
        {
            this.BackColor = lightBackgroundColor;
            titleLabel.ForeColor = lightAccentColor;
            themeToggleButton.BackColor = lightPrimaryColor;
            themeToggleButton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(lightPrimaryColor, 0.15f);
            themeToggleButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(lightPrimaryColor, 0.1f);
            backButton.BackColor = lightSecondaryColor;
            backButton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(lightSecondaryColor, 0.15f);
            backButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(lightSecondaryColor, 0.1f);
        }
        
        UpdateThemeDisplay();
    }

    private void UpdateThemeDisplay()
    {
        themeToggleButton.Text = currentTheme == Theme.Dark ? "Theme: Dark" : "Theme: Light";
    }

    private void BackButton_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    public Theme GetCurrentTheme()
    {
        return currentTheme;
    }

    /// <summary>
    /// Возвращает текущий цвет фона в зависимости от темы
    /// </summary>
    private Color backgroundColor
    {
        get => currentTheme == Theme.Dark ? darkBackgroundColor : lightBackgroundColor;
    }

    /// <summary>
    /// Возвращает текущий основной цвет в зависимости от темы
    /// </summary>
    private Color primaryColor
    {
        get => darkPrimaryColor; // Основной цвет одинаковый для обеих тем
    }

    /// <summary>
    /// Возвращает текущий вторичный цвет в зависимости от темы
    /// </summary>
    private Color secondaryColor
    {
        get => currentTheme == Theme.Dark ? darkSecondaryColor : lightSecondaryColor;
    }

    /// <summary>
    /// Возвращает текущий акцентный цвет в зависимости от темы
    /// </summary>
    private Color accentColor
    {
        get => darkAccentColor; // Акцентный цвет одинаковый для обеих тем
    }
}
