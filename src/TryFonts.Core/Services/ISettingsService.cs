using TryFonts.Core.Models;

namespace TryFonts.Core.Services;

/// <summary>
/// Persists and restores <see cref="AppSettings"/>.
/// Implementations must write to the OS user-scoped app-data location,
/// never to the application install directory.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Loads settings from storage. Returns defaults if the file does not exist
    /// or cannot be read.
    /// </summary>
    AppSettings Load();

    /// <summary>
    /// Persists settings to storage. Silently ignores write failures so the app
    /// can continue operating without crashing.
    /// </summary>
    void Save(AppSettings settings);
}
