
using System.Drawing;
using System.Windows.Forms;
using WORDLE;

namespace WORDLE;

public class Program
{
    [STAThread]
    static void Main()
    {
        // Инициализируем приложение
        ApplicationConfiguration.Initialize();
        
        // Запускаем главное меню - пользователь сначала попадает в меню
        Application.Run(new MainMenuForm());
    }
}