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
        this.BackColor = Color.FromArgb(26, 26, 26);
        create_tiles();
        create_keyboard();
        this.CenterToScreen();
    }

    #endregion

    private Panel panel_keys = new Panel();
    private Panel panel_tiles = new Panel();
    private int size_x = 900;
    private int size_y = 1100;
    private Color tile_color = Color.FromArgb(51, 51, 51);
    private Color key_color = Color.FromArgb(128, 128, 128); // Светло-серый для не введённых букв

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
                ForeColor = Color.White,
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
        }
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
                    ForeColor = Color.White,
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
            ForeColor = Color.White
        };
        if (win != "")
        {
            end_window.Text = "You won!\n" + win;
            end_window.BackColor = game.green;
        }
        else
        {
            end_window.Text = "You lost. Correct word: " + game.word.ToUpper();
            end_window.BackColor = game.red;
        }

        end_window.Visible = true;

        end_window.Size = new Size(8 * size_x / 9, size_y / 5);

        panel_end.Controls.Add(end_window);
        panel_end.Size = new Size(8 * size_x / 9, size_y / 5);
        panel_end.Location = new Point((size_x - end_window.Size.Width) / 2,
                                        (size_y - end_window.Size.Height) / 2);
        panel_end.Controls[panel_end.Controls.Count - 1].BringToFront();

        this.Controls.Add(panel_end);
        this.Controls[this.Controls.Count - 1].BringToFront();
    }
}
