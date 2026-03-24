
namespace WORDLE;

public class Program
{
    [STAThread]
    static void Main()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "words_list.txt");
        string word = random_word(path);
        Console.WriteLine(word);
        Game game = new Game(word);
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1(game));
    }

    static string random_word(string path)
    {
        System.Diagnostics.Debug.WriteLine(path);
        var lines = File.ReadAllLines(path);
        Random r = new Random();
        int randomLineNumber = r.Next(0, lines.Length);
        string word = lines[randomLineNumber];
        return word;
    }
}