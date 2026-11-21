using System.Text.Json;
using MyFirstHttp.Framework.Configuration;

namespace MyFirstHttp.Framework.Utilities;

public class JsonDeserialize
{
    private static ServerSettings _settings = Deserialize();

    public static ServerSettings GetServer => _settings;

    private static ServerSettings Deserialize()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
        try
        {
            var result = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText(jsonPath)) ?? new ServerSettings();
            return result;
        }
        catch (FileNotFoundException)
        {
            var settings = new ServerSettings();
            File.WriteAllText(jsonPath, JsonSerializer.Serialize(settings));
            return settings;
        }
        catch (JsonException)
        {
            Console.WriteLine("File is corrupt.");
            return new ServerSettings();
        }
    }

    private JsonDeserialize(){}
}
