// LFInteractive LLC. - All Rights Reserved
using Chase.WebDeploy.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chase.WebDeploy.Core.Controllers;

public class ConfigurationController
{
    private ConfigurationController()
    {
        Settings = new SettingsModel()
        {
            Port = 8256,
            Authentication = new()
            {
                Username = "wdeploy",
                Password = ""
            }
        };
        Load();
    }

    public static readonly ConfigurationController Instance = Instance ?? new();
    public SettingsModel Settings { get; private set; }

    public void Load()
    {
        if (!File.Exists(Values.Files.ApplicationConfig))
        {
            Save();
        }
        using FileStream fs = new(Values.Files.ApplicationConfig, FileMode.Open, FileAccess.Read);
        using StreamReader reader = new(fs);
        Settings = JObject.Parse(reader.ReadToEnd()).ToObject<SettingsModel>();
    }

    public void Save()
    {
        using FileStream fs = new(Values.Files.ApplicationConfig, FileMode.OpenOrCreate, FileAccess.Write);
        using StreamWriter writer = new(fs);
        writer.Write(JsonConvert.SerializeObject(Settings, Formatting.Indented));
    }
}