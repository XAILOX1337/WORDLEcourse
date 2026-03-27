using System.Text.Json;

namespace WORDLE;

/// <summary>
/// Класс для управления сохранением и загрузкой рекордов игрока
/// </summary>
public class RecordManager
{
    private static readonly string RecordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "record.json");
    
    /// <summary>
    /// Загружает рекорд из файла
    /// </summary>
    public static int LoadRecord()
    {
        if (File.Exists(RecordFilePath))
        {
            try
            {
                string json = File.ReadAllText(RecordFilePath);
                var recordData = JsonSerializer.Deserialize<RecordData>(json);
                return recordData?.Record ?? 0;
            }
            catch
            {
                return 0;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// Сохраняет рекорд в файл
    /// </summary>
    public static void SaveRecord(int record)
    {
        try
        {
            var recordData = new RecordData { Record = record };
            string json = JsonSerializer.Serialize(recordData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(RecordFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving record: " + ex.Message);
        }
    }
    
    /// <summary>
    /// Класс для сериализации данных рекорда
    /// </summary>
    private class RecordData
    {
        public int Record { get; set; }
    }
}
