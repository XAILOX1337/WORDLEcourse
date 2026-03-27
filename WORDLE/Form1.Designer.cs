#nullable disable

using System.Drawing.Drawing2D;

namespace WORDLE;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {

        this.Size = new System.Drawing.Size(size_x, size_y);
        this.Text = "WORDLE";
        this.BackColor = backgroundColor;
        create_tiles();
        create_keyboard();
        this.CenterToScreen();
    }

    #endregion

    private Panel panel_keys = new Panel();
    private Panel panel_tiles = new Panel();
    private int size_x = 900;
    private int size_y = 1100;
    
    // Dark theme colors (default)
    private Color darkBackgroundColor = Color.FromArgb(18, 18, 18);
    private Color darkTileColor = Color.FromArgb(18, 18, 19);
    private Color darkKeyColor = Color.FromArgb(129, 131, 132);

    // Light theme colors
    private Color lightBackgroundColor = Color.FromArgb(227, 227, 225);
    private Color lightTileColor = Color.FromArgb(227, 227, 225);
    private Color lightKeyColor = Color.FromArgb(180, 180, 180);

    private Color backgroundColor
    {
        get => currentTheme == Theme.Dark ? darkBackgroundColor : lightBackgroundColor;
    }

    private Color tile_color
    {
        get => currentTheme == Theme.Dark ? darkTileColor : lightTileColor;
    }

    private Color key_color
    {
        get => currentTheme == Theme.Dark ? darkKeyColor : lightKeyColor;
    }

    private void create_keyboard()
    {
        int key_width = 60;
        int key_height = 60;
        int space = 8;

        // Настраиваем панель клавиатуры
        panel_keys.Size = new Size(size_x, 3 * key_height + 3 * space);
        panel_keys.Location = new Point(0, size_y - 280);
        panel_keys.BackColor = Color.Transparent;
        panel_keys.Visible = true;

        // Первый ряд: QWERTYUIOP
        string[] row1 = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
        create_keyboard_row(row1, 0, key_width, key_height, space);

        // Второй ряд: ASDFGHJKL (с отступом влево для сходства с реальной клавиатурой)
        string[] row2 = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
        create_keyboard_row(row2, 1, key_width, key_height, space, indent_extra: -5);

        // Третий ряд: ENTER, ZXCVBNM, DELETE
        string[] row3 = { "ENTER", "Z", "X", "C", "V", "B", "N", "M", "DELETE" };
        create_keyboard_row(row3, 2, key_width, key_height, space, is_special_row: true);

        this.Controls.Add(panel_keys);
        panel_keys.BringToFront();
    }

    private void create_keyboard_row(string[] keys, int row_index, int key_width, int key_height, int space, int indent_extra = 0, bool is_special_row = false)
    {
        int row_length = keys.Length;

        // Для специального ряда считаем ширину с учётом широких клавиш
        int total_width;
        if (is_special_row)
        {
            // ENTER и DELETE шире обычных
            total_width = (row_length - 2) * key_width + 2 * (key_width * 2 + space) + (row_length - 1) * space;
        }
        else
        {
            total_width = row_length * key_width + (row_length - 1) * space;
        }

        int indent = (size_x - total_width) / 2 + indent_extra;

        for (int j = 0; j < keys.Length; j++)
        {
            string key_text = keys[j];
            Label key = new Label
            {
                Height = key_height,
                Top = (row_index * (key_height + space) + space),
                BorderStyle = BorderStyle.None,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Calibri", 16, FontStyle.Bold),
                BackColor = key_color,
                ForeColor = currentTheme == Theme.Light ? Color.Black : Color.White,
                Cursor = Cursors.Hand,
                Visible = true
            };

            if (key_text == "ENTER" || key_text == "DELETE")
            {
                key.Text = key_text;
                key.Width = key_width * 2 + space;
                key.Left = indent;
                key.Font = new Font("Calibri", 11, FontStyle.Bold);
                key.Name = key_text;
                indent += key.Width + space;
            }
            else
            {
                key.Text = key_text;
                key.Width = key_width;
                key.Left = indent;
                key.Name = key_text;
                indent += key_width + space;
            }

            key.Click += key_click;
            panel_keys.Controls.Add(key);

            // Добавляем в словарь только буквенные клавиши для подсветки
            if (key.Name != "ENTER" && key.Name != "DELETE" && !keys_dict.ContainsKey(key.Name))
            {
                keys_dict.Add(key.Name, key);
            }

            // Делаем закругленные углы клавиши
            MakeLabelRounded(key, 10);
        }
    }

    /// <summary>
    /// Делает углы Label закругленными
    /// </summary>
    private void MakeLabelRounded(Label label, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(label.Width - radius, 0, radius, radius, 270, 90);
        path.AddArc(label.Width - radius, label.Height - radius, radius, radius, 0, 90);
        path.AddArc(0, label.Height - radius, radius, radius, 90, 90);
        path.CloseAllFigures();
        label.Region = new Region(path);
    }

    private void create_tiles()
    {
        int tile_size = 90;
        int space = 15;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < game.max_length; j++)
            {
                Label tile = new Label
                {
                    Width = tile_size,
                    Height = tile_size,
                    Top = (i * (tile_size + space) + space),
                    Left = j * (tile_size + space),
                    Font = new Font("Calibri", 20, FontStyle.Bold),
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "",
                    Name = i.ToString() + j.ToString(),
                    ForeColor = currentTheme == Theme.Light ? Color.Black : Color.White,
                    BackColor = tile_color
                };
                tile.Visible = true;
                tiles_dict.Add(tile.Name, tile);
                panel_tiles.Controls.Add(tile);

            }
        }
        panel_tiles.Size = new Size(5 * (tile_size + space) + 4 * space, 6 * (tile_size + space) + 5 * space);
        panel_tiles.Location = new Point(size_x / 2 - panel_tiles.Size.Width / 2, size_y / 20);
        panel_tiles.Visible = true;
        this.Controls.Add(panel_tiles);
    }

    private void end(string win)
    {
        Panel panel_end = new Panel();
        Label end_window = new Label
        {
            Font = new Font("Calibri", 20, FontStyle.Bold),
            BorderStyle = BorderStyle.FixedSingle,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            AutoSize = false
        };
        if (win != "")
        {
            end_window.Text = "You won!\n" + win;
            end_window.BackColor = game.green;
            game.input_blocked = true;
        }
        else
        {
            end_window.Text = "You lost. Correct word: " + game.word.ToUpper();
            end_window.BackColor = game.red;
            game.input_blocked = true;
        }

        end_window.Visible = true;
        end_window.Size = new Size(8 * size_x / 9, size_y / 6);

        // Создаем панель для кнопок
        Panel buttons_panel = new Panel
        {
            Size = new Size(8 * size_x / 9, 100),
            BackColor = Color.Transparent,
            Location = new Point(0, end_window.Size.Height + 20)
        };

        // Кнопка "Сыграть еще раз" - запускает новую игру
        Button playAgainButton = new Button
        {
            Text = "Сыграть еще раз",
            Font = new Font("Calibri", 14, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = game.green,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Width = 200,
            Height = 45,
            Location = new Point((buttons_panel.Width - 420) / 2, 10)
        };

        playAgainButton.FlatAppearance.BorderSize = 0;
        playAgainButton.Click += PlayAgainButton_Click;

        // Кнопка "Выйти из игры" - закрывает приложение
        Button exitButton = new Button
        {
            Text = "Выйти из игры",
            Font = new Font("Calibri", 14, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = game.red,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Width = 200,
            Height = 45,
            Location = new Point((buttons_panel.Width - 420) / 2 + 220, 10)
        };
        exitButton.FlatAppearance.BorderSize = 0;
        exitButton.Click += ExitButton_Click;

        buttons_panel.Controls.Add(playAgainButton);
        buttons_panel.Controls.Add(exitButton);

        panel_end.Controls.Add(end_window);
        panel_end.Controls.Add(buttons_panel);
        panel_end.Size = new Size(8 * size_x / 9, end_window.Size.Height + buttons_panel.Size.Height + 30);
        panel_end.Location = new Point((size_x - panel_end.Size.Width) / 2,
                                        (size_y - panel_end.Size.Height) / 2);
        panel_end.BackColor = Color.Transparent;

        this.Controls.Add(panel_end);
        this.Controls[this.Controls.Count - 1].BringToFront();
    }

    /// <summary>
    /// Обработчик кнопки "Сыграть еще раз" - запускает новую игру с новым словом
    /// </summary>
    private void PlayAgainButton_Click(object sender, EventArgs e)
    {
        // Находим панель с результатом и удаляем её
        Control panel_end = this.Controls.Cast<Control>().FirstOrDefault(c => c is Panel && c.Controls.Count > 0);
        if (panel_end != null)
        {
            this.Controls.Remove(panel_end);
            panel_end.Dispose();
        }

        // Загружаем новое случайное слово
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "words_list.txt");
        string newWord = GetRandomWord(path);
        Console.WriteLine("Новое слово: " + newWord);

        // Создаем новый объект игры с текущей темой
        this.game = new Game(newWord, currentTheme);

        // Сбрасываем состояние игры
        this.isAnimating = false;

        // Очищаем и пересоздаем плитки (сбрасываем текст и цвет)
        foreach (var tile in this.tiles_dict.Values)
        {
            tile.Text = "";
            tile.BackColor = this.tile_color;
        }

        // Сбрасываем цвета клавиш
        foreach (var key in this.keys_dict.Values)
        {
            key.BackColor = this.key_color;
        }

        // Разблокируем ввод
        this.game.input_blocked = false;
    }

    /// <summary>
    /// Обработчик кнопки "Выйти из игры" - закрывает игру и возвращает в главное меню
    /// </summary>
    private void ExitButton_Click(object sender, EventArgs e)
    {
        // Закрываем форму игры - это вернёт пользователя в главное меню
        this.Close();
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
