using System.Text.Json;

namespace Httpd.CLI;

public class Config
{
    public int Port { get; }
    public bool DirectoryListing { get; }
    public string[] Extensions { get; private set; }
    private JsonElement _configJson;

    public Config()
    {
        ReadConfigFile();
        Port = SetPort();
        DirectoryListing = SetDirectoryListing();
        Extensions = InitializeExtensions();
        SetExtensions();
    }

    private void ReadConfigFile()
    {
        var text = File.ReadAllText(Directory.GetCurrentDirectory() + "/config.json");
        var doc = JsonDocument.Parse(text);
        _configJson = doc.RootElement;
    }

    private int SetPort()
    {
        foreach (var jsonProperty in _configJson.EnumerateObject()
                     .Where(jsonProperty => jsonProperty.Name.Equals("Port")))
        {
            return jsonProperty.Value.GetInt32();
        }

        return 3000;
    }

    private bool SetDirectoryListing()
    {
        foreach (var jsonProperty in _configJson.EnumerateObject()
                     .Where(jsonProperty => jsonProperty.Name.Equals("Directory listing")))
        {
            return jsonProperty.Value.GetBoolean();
        }

        return false;
    }

    private static string[] InitializeExtensions()
    {
        return new[]
        {
            ".html",
            ".css",
            ".js",
            ".jpg",
            ".png",
            ".gif",
            ".mov"
        };
    }

    private void SetExtensions()
    {
        foreach (var jsonProperty in _configJson.EnumerateObject()
                     .Where(jsonProperty => jsonProperty.Name.Equals("Extensions")))
        {
            Extensions = jsonProperty.Value.Deserialize<string[]>()!;
        }

        if (Extensions.Length <= 0)
        {
            Extensions = InitializeExtensions();
        }
    }
}