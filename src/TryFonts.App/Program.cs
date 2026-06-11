using Avalonia;

namespace TryFonts.App;

internal static class Program
{
    // Avalonia requires STA on Windows; harmless on macOS.
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp(args).StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Public so integration tests and previewer tooling can call it without
    /// fully launching the desktop lifetime.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp(string[]? args = null)
    {
        // Parse --synthetic-fonts <n> before building so App.axaml.cs can read it.
        AppStartupArgs.Parse(args ?? []);

        return AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
