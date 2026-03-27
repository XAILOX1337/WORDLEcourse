using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WORDLE;

/// <summary>
/// Форма главного меню игры Wordle
/// </summary>
public class MainMenuForm : Form
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

    // UI elements
    private Label titleLabel;
    private Button playButton;
    private Button settingsButton;
    private Button helpButton;
    private Label recordLabel;

    public MainMenuForm()
    {
        InitializeMainMenu();
    }

    private void InitializeMainMenu()
    {
        // Настройка формы
        this.Size = new Size(size_x, size_y);
        this.Text = "WORDLE";
        this.BackColor = backgroundColor;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        // Создаем заголовок WORDLE большим шрифтом
        titleLabel = new Label
        {
            Text = "WORDLE",
            Font = new Font("Calibri", 72, FontStyle.Bold),
            ForeColor = accentColor,
            BackColor = Color.Transparent,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter
        };
        // Центрируем заголовок - вычисляем позицию после создания
        titleLabel.Location = new Point(
            (size_x - titleLabel.PreferredWidth) / 2,
            size_y / 8
        );

        // Создаем метку рекорда
        int record = RecordManager.LoadRecord();
        recordLabel = new Label
        {
            Text = $"Рекорд: {record}",
            Font = new Font("Calibri", 18, FontStyle.Bold),
            ForeColor = currentTheme == Theme.Light ? Color.Black : Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter
        };
        recordLabel.Location = new Point(
            (size_x - recordLabel.PreferredWidth) / 2,
            size_y / 12
        );

        // Создаем кнопку "Начать игру"
        playButton = CreateMenuButton("Начать игру", primaryColor);
        playButton.Location = new Point(
            (size_x - playButton.Width) / 2,
            size_y / 3
        );
        playButton.Click += PlayButton_Click;

        // Создаем кнопку "Настройки"
        settingsButton = CreateMenuButton("Настройки", secondaryColor);
        settingsButton.Location = new Point(
            (size_x - settingsButton.Width) / 2,
            size_y / 3 + 70
        );
        settingsButton.Click += SettingsButton_Click;

        // Создаем кнопку "Справка"
        helpButton = CreateMenuButton("Справка", secondaryColor);
        helpButton.Location = new Point(
            (size_x - helpButton.Width) / 2,
            size_y / 3 + 140
        );
        helpButton.Click += HelpButton_Click;

        // Добавляем все элементы на форму
        this.Controls.Add(titleLabel);
        this.Controls.Add(playButton);
        this.Controls.Add(settingsButton);
        this.Controls.Add(helpButton);
        this.Controls.Add(recordLabel);
        recordLabel.BringToFront();
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

        // Убираем стандартные границы и делаем плоский стиль
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.15f);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.1f);

        // Делаем закругленные углы кнопки
        button.Click += (s, e) => MakeButtonRounded(button);
        // Вызываем сразу после создания
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

    private void PlayButton_Click(object? sender, EventArgs e)
    {
        // Запускаем игру - скрываем меню и открываем Form1
        this.Hide();

        // Создаем новую игру со случайным словом
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "words_list.txt");
        string word = GetRandomWord(path);


        Game game = new Game(word, currentTheme);
        Form1 gameForm = new Form1(game, currentTheme);
        gameForm.Show();
        Console.WriteLine("Загаданное слово: " + word);


        // Когда игра закроется, показываем меню снова
        gameForm.FormClosed += (s, args) =>
        {
            this.Show();
            this.Activate();
            // Обновляем метку рекорда после возврата из игры
            UpdateRecordLabel();
        };
    }

    private void SettingsButton_Click(object? sender, EventArgs e)
    {

        // Открываем форму настроек
        SettingsForm settingsForm = new SettingsForm(currentTheme);
        settingsForm.ShowDialog();

        // После закрытия настроек применяем тему
        if (settingsForm.GetCurrentTheme() != currentTheme)
        {
            currentTheme = settingsForm.GetCurrentTheme();
            ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        Color bgColor = currentTheme == Theme.Dark ? darkBackgroundColor : lightBackgroundColor;
        Color accentClr = currentTheme == Theme.Dark ? darkAccentColor : lightAccentColor;
        Color secColor = currentTheme == Theme.Dark ? darkSecondaryColor : lightSecondaryColor;

        this.BackColor = bgColor;
        titleLabel.ForeColor = accentClr;
        recordLabel.ForeColor = currentTheme == Theme.Light ? Color.Black : Color.White;
        playButton.BackColor = primaryColor;
        settingsButton.BackColor = secColor;
        helpButton.BackColor = secColor;
    }
    
    private void UpdateRecordLabel()
    {
        int record = RecordManager.LoadRecord();
        recordLabel.Text = $"Рекорд: {record}";
    }

    private void HelpButton_Click(object? sender, EventArgs e)
    {
        // Заглушка для будущего функционала справки
        MessageBox.Show(
            "Правила игры Wordle:\n\n" +
            "• Угадайте слово, состоящее из 5 букв за 6 попыток\n" +
            "• Вводить можно только реальные слова\n" +
            "• Зеленый цвет - буква на правильном месте\n" +
            "• Желтый цвет - буква есть в слове, но не на своем месте\n" +
            "• Серый цвет - буквы нет в слове\n\n",
            "Справка",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
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

    /// <summary>
    /// Выбирает случайное слово из файла словаря
    /// </summary>
    private string GetRandomWord(string path)
    {
        var lines = File.ReadAllLines(path);
        Random r = new Random();
        int randomLineNumber = r.Next(0, lines.Length);
        return lines[randomLineNumber].Trim().ToUpper();
    }
}
