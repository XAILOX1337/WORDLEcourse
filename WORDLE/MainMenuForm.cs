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
    // Более приятная цветовая гамма
    private Color backgroundColor = Color.FromArgb(18, 18, 19); 
    private Color primaryColor = Color.FromArgb(83, 141, 78); 
    private Color secondaryColor = Color.FromArgb(129, 131, 132); 
    private Color accentColor = Color.FromArgb(83, 141, 78); 

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
        Label titleLabel = new Label
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

        // Создаем кнопку "Начать игру"
        Button playButton = CreateMenuButton("Начать игру", primaryColor);
        playButton.Location = new Point(
            (size_x - playButton.Width) / 2,
            size_y / 3
        );
        playButton.Click += PlayButton_Click;

        // Создаем кнопку "Настройки" (зарезервировано для будущего функционала)
        Button settingsButton = CreateMenuButton("Настройки", secondaryColor);
        settingsButton.Location = new Point(
            (size_x - settingsButton.Width) / 2,
            size_y / 3 + 70
        );
        settingsButton.Click += SettingsButton_Click;

        // Создаем кнопку "Справка" (зарезервировано для будущего функционала)
        Button helpButton = CreateMenuButton("Справка", secondaryColor);
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


        Game game = new Game(word);
        Form1 gameForm = new Form1(game);
        gameForm.Show();
        Console.WriteLine("Загаданное слово: " + word);


        // Когда игра закроется, показываем меню снова
        gameForm.FormClosed += (s, args) =>
        {
            this.Show();
            this.Activate();
        };
    }

    private void SettingsButton_Click(object? sender, EventArgs e)
    {
        // Заглушка для будущего функционала настроек
        MessageBox.Show(
            "Функционал настроек будет добавлен в следующей версии.",
            "Настройки",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void HelpButton_Click(object? sender, EventArgs e)
    {
        // Заглушка для будущего функционала справки
        MessageBox.Show(
            "Правила игры Wordle:\n\n" +
            "• Угадайте слово за 6 попыток\n" +
            "• Зеленый цвет - буква на правильном месте\n" +
            "• Желтый цвет - буква есть в слове, но не на своем месте\n" +
            "• Серый цвет - буквы нет в слове\n\n" +
            "Полная справка будет добавлена в следующей версии.",
            "Справка",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
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
