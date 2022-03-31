using System.Configuration;

namespace Httpd.CLI;

public class Config
{
    public int Port { get; private set; }
    public bool DirectoryListing { get; private set; }
    public string[] Extensions { get; private set; }

    public Config()
    {
        Port = 3000;
        DirectoryListing = false;
        Extensions = InitializeExtensions();
        ReadConfigFile();
    }

    private void ReadConfigFile()
    {
        if (!ConfigurationManager.AppSettings.HasKeys())
        {
            return;
        }

        var keys = ConfigurationManager.AppSettings.AllKeys;
        foreach (var key in keys)
        {
            switch (key)
            {
                case "Port":
                    SetPort(ConfigurationManager.AppSettings.Get(key));
                    break;
                case "Directory listing":
                    SetDirectoryListing(ConfigurationManager.AppSettings.Get(key));
                    break;
                case "Extensions":
                    SetExtensions(ConfigurationManager.AppSettings.Get(key));
                    break;
            }
        }
    }

    private void SetPort(string? value)
    {
        if (!int.TryParse(value, out var port)) return;
        Port = port;
    }

    private void SetDirectoryListing(string? value)
    {
        DirectoryListing = value!.Equals("enabled");
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

    private void SetExtensions(string? values)
    {
        var extensions = values!.Split();
        if (extensions.Any(extension => !Extensions.Contains(extension)))
        {
            return;
        }

        Extensions = extensions;
    }
}