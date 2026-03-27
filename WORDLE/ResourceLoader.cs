using System.Reflection;

namespace WORDLE;

public static class ResourceLoader
{
    public static string[] GetWordsList()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("WORDLE.words_list.txt") 
            ?? throw new FileNotFoundException("words_list.txt not found in resources");
        using var reader = new StreamReader(stream);
        
        var words = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            string trimmed = line.Trim().ToUpper();
            if (!string.IsNullOrEmpty(trimmed))
            {
                words.Add(trimmed);
            }
        }
        return words.ToArray();
    }
}
