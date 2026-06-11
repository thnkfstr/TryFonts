namespace TryFonts.Core.Models;

/// <summary>
/// User preferences that are persisted between sessions.
/// <para>
/// NOTE: <c>PreviewText</c> is intentionally NOT persisted.
/// Every launch must start with <see cref="Services.PreviewTextPresets.BaseSampleText"/>.
/// </para>
/// </summary>
public sealed class AppSettings
{
    public double FontSize { get; set; } = 24.0;
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public SearchMode SearchMode { get; set; } = SearchMode.Contains;
    public SortMode SortMode { get; set; } = SortMode.NameAZ;

    // Window geometry — NaN means "let the OS decide"
    public double WindowWidth { get; set; } = 1200;
    public double WindowHeight { get; set; } = 800;
    public double WindowX { get; set; } = double.NaN;
    public double WindowY { get; set; } = double.NaN;

    // Schema version for future migrations
    public int SchemaVersion { get; set; } = 1;
}
