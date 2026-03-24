using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WORDLE
{
    public static class Animations
    {
        private const int FlipDuration = 300;
        private const int ShakeDuration = 400;
        private const int KeyColorDuration = 200;

        /// <summary>
        /// Анимация переворота плитки с изменением цвета (симметрично к центру)
        /// </summary>
        public static async Task FlipTileAsync(Label tile, Color newColor, int delay = 0)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            int steps = 10;
            int originalWidth = tile.Width;
            int originalLeft = tile.Left;
            Font originalFont = tile.Font;

            // Сжимаем по ширине симметрично к центру (эффект поворота)
            for (int i = 0; i < steps / 2; i++)
            {
                int newWidth = originalWidth * (steps - 2 * i) / steps;
                int diff = originalWidth - newWidth;
                tile.Width = newWidth;
                tile.Left = originalLeft + diff / 2;
                
                // Уменьшаем шрифт пропорционально
                float fontScale = (float)(steps - 2 * i) / steps;
                int newFontSize = Math.Max(1, (int)(originalFont.Size * fontScale));
                tile.Font = new Font(originalFont.FontFamily, newFontSize, originalFont.Style);
                
                await Task.Delay(FlipDuration / steps);
            }

            // Меняем цвет в середине переворота (когда плитка "невидима")
            tile.BackColor = newColor;
            tile.ForeColor = Color.White;

            // Возвращаем ширину симметрично
            for (int i = steps / 2; i < steps; i++)
            {
                int newWidth = originalWidth * (2 * i - steps + 2) / steps;
                int diff = originalWidth - newWidth;
                tile.Width = newWidth;
                tile.Left = originalLeft + diff / 2;
                
                // Увеличиваем шрифт пропорционально
                float fontScale = (float)(2 * i - steps + 2) / steps;
                int newFontSize = Math.Max(1, (int)(originalFont.Size * fontScale));
                tile.Font = new Font(originalFont.FontFamily, newFontSize, originalFont.Style);
                
                await Task.Delay(FlipDuration / steps);
            }

            tile.Width = originalWidth;
            tile.Left = originalLeft;
            tile.Font = originalFont;
        }

        /// <summary>
        /// Анимация появления буквы
        /// </summary>
        public static async Task PopLetterAsync(Label tile)
        {
            int steps = 8;
            Font originalFont = tile.Font;
            
            for (int i = 0; i < steps; i++)
            {
                float scale = 0.5f + 0.5f * i / steps;
                int newSize = (int)(originalFont.Size * scale);
                tile.Font = new Font(originalFont.FontFamily, newSize, originalFont.Style);
                await Task.Delay(30);
            }
            
            tile.Font = originalFont;
        }

        /// <summary>
        /// Анимация тряски для неправильного слова
        /// </summary>
        public static async Task ShakeAsync(List<Label> tiles)
        {
            int shakeAmount = 5;
            int shakeCount = 6;
            int originalLeft = tiles[0].Left;

            for (int i = 0; i < shakeCount; i++)
            {
                int offset = (i % 2 == 0 ? shakeAmount : -shakeAmount);
                foreach (var tile in tiles)
                {
                    tile.Left += offset;
                }
                await Task.Delay(ShakeDuration / shakeCount);
            }

            // Возвращаем на место
            foreach (var tile in tiles)
            {
                tile.Left = originalLeft + (tile.Name.Length > 1 ? int.Parse(tile.Name.Substring(1)) * 105 : 0);
            }
        }

        /// <summary>
        /// Плавное изменение цвета клавиши
        /// </summary>
        public static async Task FadeKeyColorAsync(Label key, Color newColor)
        {
            Color startColor = key.BackColor;
            int steps = 10;

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                int r = (int)(startColor.R + (newColor.R - startColor.R) * t);
                int g = (int)(startColor.G + (newColor.G - startColor.G) * t);
                int b = (int)(startColor.B + (newColor.B - startColor.B) * t);
                key.BackColor = Color.FromArgb(r, g, b);
                await Task.Delay(KeyColorDuration / steps);
            }

            key.BackColor = newColor;
        }

        /// <summary>
        /// Анимация появления сообщения
        /// </summary>
        public static async Task FadeInAsync(Control control, int duration = 300)
        {
            control.Visible = true;
            int steps = 10;
            for (int i = 0; i <= steps; i++)
            {
                float alpha = (float)i / steps;
                // Для Windows Forms нет прямой поддержки прозрачности,
                // но можно использовать таймер для задержки
                await Task.Delay(duration / steps);
            }
        }
    }
}
