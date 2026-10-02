using System.Net;
using System.Text.Json;
using MyFirstHttp.Framework.Configuration;

namespace MyFirstHttp.Framework.Utilities;

public class JsonDeserialize
{
    private static ServerSettings _settings = Deserialize();
    
    public static Server GetServer => _settings;

    private static ServerSettings Deserialize()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
        try
        {
            var result =  JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText(jsonPath)) ?? new ServerSettings();

            if (!_settings.Server.Path.StartsWith("/"))
            {
                _settings.Server.Path = "/" + _settings.Server.Path;
            }

            if (!_settings.Server.Path.EndsWith("/"))
            {
                _settings.Server.Path += "/";
            }
            
            return result;
        }
        catch (FileNotFoundException)
        {
            var settingString = JsonSerializer.Serialize(_settings);
            File.WriteAllText(jsonPath, settingString);
            return new ServerSettings();
        }
        catch (JsonException)
        {
            Console.WriteLine("File is corrupt.");
            return new ServerSettings();
        }
        
    }

    private JsonDeserialize(){}

}