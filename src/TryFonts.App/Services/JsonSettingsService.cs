using System.Text.Json;
using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.App.Services;

/// <summary>
/// Persists <see cref="AppSettings"/> as a JSON file in the user's application-data
/// directory. Write failures are swallowed silently so the app can always run.
/// </summary>
public sealed class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    private readonly string _settingsPath;

    public JsonSettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _settingsPath = Path.Combine(appData, "TryFonts", "settings.json");
    }

    /// <summary>Exposed for unit testing — allows an explicit path.</summary>
    internal JsonSettingsService(string path) => _settingsPath = path;

    public AppSettings Load()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                if (settings is not null)
                    return settings;
            }
        }
        catch
        {
            // Return defaults on any read/parse error
        }

        return new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        try
        {
            var dir = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings, JsonOptions));
        }
        catch
        {
            // Silently ignore write failures; the app continues without saved settings
        }
    }
}
