using System.Drawing;
using System.Windows.Forms;
using WORDLE;

namespace WORDLE;

public partial class Form1 : Form
{
    private Game game;
    private Dictionary<String, Label> keys_dict = new Dictionary<string, Label>();
    private Dictionary<String, Label> tiles_dict = new Dictionary<string, Label>();
    private bool isAnimating = false;
    private bool isKeyboardAnimating = false;
    private Theme currentTheme = Theme.Dark;
    private Label? recordLabel;

    public Form1(Game game, Theme theme = Theme.Dark)
    {
        this.game = game;
        this.currentTheme = theme;
        InitializeComponent();
        
        // Создаем и добавляем метку рекорда
        create_record_label();

        // Позволяем форме перехватывать нажатия клавиш
        this.KeyPreview = true;
        // Подписываемся на событие нажатия
        this.KeyDown += new KeyEventHandler(Form1_KeyDown);
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        // Обработка ENTER
        if (e.KeyCode == Keys.Enter)
        {
            processKey("ENTER");
        }
        // Обработка Backspace
        else if (e.KeyCode == Keys.Back)
        {
            processKey("DELETE");
        }
        // Обработка букв A-Z
        else if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
        {
            processKey(e.KeyCode.ToString());
        }
    }


    private async void processKey(string keyText)
    {
        if (isAnimating) return;

        try
        {
            if (keyText == "ENTER")
            {
                EndType end_type = game.enter();
                if (end_type == EndType.CorrectWord)
                {
                    await animate_row_flip();
                    update_key_colors();
                    end(game.word);
                }
                else if (end_type == EndType.FalseWordEnd)
                {
                    await animate_row_flip();
                    update_key_colors();
                    end("");
                }
                else if (end_type == EndType.FalseWordLength)
                {
                    // Слово слишком короткое - показываем сообщение и разблокируем ввод
                    await animate_shake();
                    show_message("Not enough letters!", game.red);
                    game.input_blocked = false;
                }
                else if (end_type == EndType.FalseWordNotInDictionary)
                {
                    // Слова нет в словаре - показываем сообщение и разблокируем ввод
                    await animate_shake();
                    show_message("Word not in dictionary!", game.red);
                    game.input_blocked = false;
                }
                else if (end_type == EndType.FalseWordContinue)
                {
                    // Слово принято, но не угадано - обновляем цвета клавиш
                    await animate_row_flip();
                    update_key_colors();
                }
                return;
            }

            if (keyText == "DELETE")
            {
                game.delete();
                return;
            }

            if (game.input_blocked) return;

            // Находим нужный Label для отображения буквы
            string tileKey = game.row.ToString() + game.column.ToString();
            if (tiles_dict.ContainsKey(tileKey))
            {
                Label tile = tiles_dict[tileKey];
                Label fakeKey = new Label { Text = keyText };
                game.enter_char(fakeKey, tile);

                // Анимация появления буквы
                await Animations.PopLetterAsync(tile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    private void key_click(object? sender, EventArgs e)
    {
        Label key = (Label)sender!;
        processKey(key.Text);
    }

    private void show_message(string message, Color backColor)
    {
        Panel panel_msg = new Panel();
        Label msg_label = new Label
        {
            Font = new Font("Calibri", 16, FontStyle.Bold),
            BorderStyle = BorderStyle.FixedSingle,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            BackColor = backColor,
            Text = message,
            AutoSize = false
        };

        msg_label.Size = new Size(8 * size_x / 9, size_y / 10);
        panel_msg.Size = new Size(8 * size_x / 9, size_y / 10);
        panel_msg.Location = new Point((size_x - msg_label.Size.Width) / 2,
                                        (size_y - msg_label.Size.Height) / 2);

        panel_msg.Controls.Add(msg_label);
        this.Controls.Add(panel_msg);
        this.Controls[this.Controls.Count - 1].BringToFront();

        // Удалить сообщение через 1.5 секунды
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        timer.Interval = 1500;
        timer.Tick += (s, args) =>
        {
            timer.Stop();
            this.Controls.Remove(panel_msg);
            panel_msg.Dispose();
        };
        timer.Start();
    }

    public async void update_key_colors()
    {
        // Блокируем кнопку "Сыграть еще раз" на время анимации клавиш
        isKeyboardAnimating = true;

        try
        {
            // Получаем цвета для каждой буквы из игры
            Dictionary<string, Color> letter_colors = game.get_letter_colors();

            foreach (var kvp in letter_colors)
            {
                string letter = kvp.Key;
                Color color = kvp.Value;

                if (keys_dict.ContainsKey(letter))
                {
                    Label key = keys_dict[letter];
                    // Не обновляем цвет если клавиша уже имеет более приоритетный цвет (зелёный > жёлтый > тёмно-серый)
                    if (color == game.green ||
                        (color == game.yellow && key.BackColor != game.green) ||
                        (color == game.currentKeyGrey && key.BackColor != game.green && key.BackColor != game.yellow))
                    {
                        // Плавная анимация изменения цвета
                        await Animations.FadeKeyColorAsync(key, color);
                    }
                }
            }
        }
        finally
        {
            // Разблокируем после завершения всех анимаций
            isKeyboardAnimating = false;
        }
    }

    private async Task animate_row_flip()
    {
        isAnimating = true;
        int current_row = game.row - 1;

        // Получаем плитки текущего ряда и их цвета из Game
        List<Label> row_tiles = new List<Label>();
        List<Color> tile_colors = game.get_current_row_colors();

        for (int i = 0; i < game.max_length; i++)
        {
            string tileKey = current_row.ToString() + i.ToString();
            if (tiles_dict.ContainsKey(tileKey))
            {
                row_tiles.Add(tiles_dict[tileKey]);
            }
        }

        // Переворачиваем плитки по очереди с задержкой, каждая со своим цветом
        for (int i = 0; i < row_tiles.Count; i++)
        {
            Label tile = row_tiles[i];
            await Task.Delay(i * 150); // Задержка перед началом анимации
            Color newColor = tile_colors[i]; // Берём цвет из Game
            await Animations.FlipTileAsync(tile, newColor, 0);
            tile.BackColor = newColor; // Применяем цвет после анимации
        }

        isAnimating = false;
    }

    private async Task animate_shake()
    {
        isAnimating = true;
        int current_row = game.row;

        // Получаем плитки текущего ряда
        List<Label> row_tiles = new List<Label>();
        for (int i = 0; i < game.max_length; i++)
        {
            string tileKey = current_row.ToString() + i.ToString();
            if (tiles_dict.ContainsKey(tileKey))
            {
                row_tiles.Add(tiles_dict[tileKey]);
            }
        }

        await Animations.ShakeAsync(row_tiles);
        isAnimating = false;
    }
    
    private void create_record_label()
    {
        recordLabel = new Label
        {
            Text = $"Победы: {game.currentStreak} | Рекорд: {game.record}",
            Font = new Font("Calibri", 14, FontStyle.Bold),
            ForeColor = currentTheme == Theme.Light ? Color.Black : Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point((size_x - 200) / 2, size_y / 20 + 100)
        };
        this.Controls.Add(recordLabel);
    }
    
    private void update_record_label()
    {
        if (recordLabel != null)
        {
            recordLabel.Text = $"Победы: {game.currentStreak} | Рекорд: {game.record}";
        }
    }
}
