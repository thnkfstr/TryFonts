namespace TryFonts.App;

/// <summary>
/// Parsed command-line arguments available to the rest of the app.
/// </summary>
internal static class AppStartupArgs
{
    /// <summary>
    /// When greater than zero, activates synthetic font mode (development/test only).
    /// This count is added to the real discovered fonts.
    /// </summary>
    public static int SyntheticFontCount { get; private set; }

    public static void Parse(string[] args)
    {
        SyntheticFontCount = 0;
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--synthetic-fonts" &&
                int.TryParse(args[i + 1], out var count) &&
                count > 0)
            {
                SyntheticFontCount = count;
                break;
            }
        }
    }
}
