using System.Drawing;
using System.Windows.Forms;
using WORDLE;

public class Game
{
    public int max_words = 6;
    public int max_length = 5;
    public int row = 0;
    public int column = 0;
    public bool input_blocked = false;
    private string current_word = "";
    public string word;
    // Обновленная цветовая гамма - более приятные цвета
    public Color yellow = Color.FromArgb(181, 159, 59); // Приятный желтый
    public Color green = Color.FromArgb(83, 141, 78); // Зеленый
    public Color red = Color.FromArgb(236, 19, 19); // Красный для поражения
    public Color key_grey = Color.FromArgb(58, 58, 60); // Тёмно-серый для не угаданных букв (dark theme)
    public Color light_key_grey = Color.FromArgb(180, 180, 180); // Светло-серый для не угаданных букв (light theme)
    
    private Theme currentTheme = Theme.Dark;

    private List<Label> letters_list = new List<Label>();
    private HashSet<string> valid_words = new HashSet<string>();
    private Dictionary<string, Color> keyboard_colors = new Dictionary<string, Color>();
    private List<Color> current_row_colors = new List<Color>(); // Цвета для текущего ряда

    public Game(string word, Theme theme = Theme.Dark)
    {
        this.word = word;
        this.currentTheme = theme;
        load_dictionary();
    }
    
    /// <summary>
    /// Возвращает текущий цвет для серых букв в зависимости от темы
    /// </summary>
    public Color currentKeyGrey
    {
        get => currentTheme == Theme.Dark ? key_grey : light_key_grey;
    }

    private void load_dictionary()
    {
        // valid_words уже инициализирован в объявлении поля
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "words_list.txt");
        try
        {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                string trimmed = line.Trim().ToUpper();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    valid_words.Add(trimmed);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Error loading dictionary: " + ex.Message);
        }
    }

    public bool is_valid_word(string word_to_check)
    {
        return valid_words.Contains(word_to_check.ToUpper());
    }

    public void enter_char(Label key, Label tile)
    {
        tile.Text = key.Text;
        current_word += key.Text;
        letters_list.Add(tile);
        column++;
        
        // Блокируем ввод после 5 букв до нажатия Enter
        if (column == max_length)
        {
            input_blocked = true;
        }
    }

    public EndType enter()
    {
        // Проверка: слово должно быть длиной 5 букв
        if (current_word.Length < max_length)
        {
            return EndType.FalseWordLength;
        }

        // Проверка: слово должно существовать в словаре
        if (!is_valid_word(current_word))
        {
            return EndType.FalseWordNotInDictionary;
        }

        if (check_word()) 
        {
            // Победа - переход на следующую строку
            row++;
            column = 0;
            input_blocked = false;
            letters_list.Clear();
            current_word = "";
            return EndType.CorrectWord;
        }

        // Слово корректное, но не угадано - переход на следующую строку
        row++;
        column = 0;
        input_blocked = false;
        letters_list.Clear();
        current_word = "";
        
        if (row >= max_words) return EndType.FalseWordEnd;

        return EndType.FalseWordContinue;
    }
    
    public void delete()
    {
        // Нельзя удалять если мы на новой строке (column == 0)
        if (column == 0)
        {
            return;
        }

        if (letters_list.Count > 0)
        {
            letters_list[letters_list.Count - 1].Text = "";
            letters_list.RemoveAt(letters_list.Count - 1);
            current_word = current_word.Remove(current_word.Length - 1);
            column--;

            // Разблокируем ввод если удалили букву
            input_blocked = false;
        }
    }

    private bool check_word()
    {
        Dictionary<string, int> letter_count = new Dictionary<string, int>();
        int correct_letters = 0;
        current_row_colors.Clear(); // Очищаем цвета перед проверкой
        
        // Первый проход: определяем цвета для каждой позиции
        for (int i = 0; i < max_length; i++)
        {
            string letter = letters_list[i].Text;
            if (!letter_count.ContainsKey(letter)) letter_count[letter] = 0;

            Color tileColor = currentKeyGrey; // По умолчанию серый (зависит от темы)
            
            if (letter == word[i].ToString())
            {
                correct_letters++;
                tileColor = green;
                letter_count[letter]++;
            }
            else if (word.Contains(letter))
            {
                if (letter_count[letter] < word.Count(c => c.ToString() == letter))
                {
                    tileColor = yellow;
                    letter_count[letter]++;
                }
            }
            
            current_row_colors.Add(tileColor);
        }

        // Второй проход: корректируем жёлтые буквы, которые встречаются больше раз, чем в загаданном слове
        for (int i = 0; i < max_length; i++)
        {
            string letter = letters_list[i].Text;
            int letterCountInWord = word.Count(c => c.ToString() == letter);
            
            // Если буква не зелёная, но уже есть нужное количество таких букв
            if (current_row_colors[i] == yellow)
            {
                int greenYellowCount = 0;
                for (int j = 0; j < max_length; j++)
                {
                    if (letters_list[j].Text == letter && (current_row_colors[j] == green || current_row_colors[j] == yellow))
                    {
                        greenYellowCount++;
                    }
                }
                if (greenYellowCount > letterCountInWord)
                {
                    // Находим лишние жёлтые и делаем их серыми (начиная с конца)
                    int excess = greenYellowCount - letterCountInWord;
                    for (int j = max_length - 1; j >= 0 && excess > 0; j--)
                    {
                        if (letters_list[j].Text == letter && current_row_colors[j] == yellow)
                        {
                            current_row_colors[j] = currentKeyGrey;
                            excess--;
                        }
                    }
                }
            }
        }

        current_word = "";

        // Обновляем цвета клавиатуры после проверки
        update_keyboard_colors();

        return correct_letters == max_length;
    }

    public List<Color> get_current_row_colors()
    {
        return new List<Color>(current_row_colors);
    }

    public Dictionary<string, Color> get_letter_colors()
    {
        // Возвращаем накопленные цвета клавиатуры
        return new Dictionary<string, Color>(keyboard_colors);
    }

    public void update_keyboard_colors()
    {
        // Обновляем цвета клавиш на основе current_row_colors
        for (int i = 0; i < letters_list.Count && i < current_row_colors.Count; i++)
        {
            string letter = letters_list[i].Text;
            Color tileColor = current_row_colors[i];

            // Если буква ещё не добавлена или новый цвет более приоритетный
            if (!keyboard_colors.ContainsKey(letter))
            {
                keyboard_colors[letter] = tileColor;
            }
            else
            {
                // Приоритет: зелёный > жёлтый > серый
                if (tileColor == green)
                {
                    keyboard_colors[letter] = green;
                }
                else if (tileColor == yellow && keyboard_colors[letter] != green)
                {
                    keyboard_colors[letter] = yellow;
                }
                else if (tileColor == currentKeyGrey && keyboard_colors[letter] != green && keyboard_colors[letter] != yellow)
                {
                    keyboard_colors[letter] = currentKeyGrey;
                }
            }
        }
    }
}