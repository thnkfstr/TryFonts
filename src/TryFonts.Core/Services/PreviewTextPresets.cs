namespace TryFonts.Core.Services;

/// <summary>A named block of sample text used to preview font families.</summary>
public sealed record PreviewTextPreset(string Name, string Text);

/// <summary>
/// Built-in preview text presets.
/// <para>
/// <see cref="BaseSampleText"/> is the mandatory startup text. It must be loaded on
/// every fresh launch regardless of what the user typed in a prior session.
/// </para>
/// </summary>
public static class PreviewTextPresets
{
    /// <summary>
    /// The canonical base sample. Covers uppercase, lowercase, digits, and common
    /// punctuation/symbols in a single memorable sentence.
    /// </summary>
    public const string BaseSampleText =
        "*The quick brown fox jumps over 10 of the 2,345 lazy dogs @ the farm" +
        " - starting with #6 & costing $7 (plus $0.89 tax?)!";

    /// <summary>All built-in presets in display order.</summary>
    public static readonly IReadOnlyList<PreviewTextPreset> All =
    [
        new("Base sample", BaseSampleText),
        new("Alphabet & digits",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz 0123456789"),
        new("Upper vs lower",
            "UPPERCASE lowercase MiXeD CaSe"),
        new("Punctuation & symbols",
            "! @ # $ % ^ & * ( ) - _ = + [ ] { } | ; : ' \" , . < > ? / ` ~ \\"),
        new("Typography",
            "“Hello” ‘world’ — em-dash – en-dash" +
            " £€¥¢ ½ ¼ ¾ × ÷ ≠ ≈ ∞"),
        new("Latin extended",
            "ÀÁÂÃÄÅÆÇÈÉÊË" +
            " ÌÍÎÏÑÒÓÔÕÖØÙÚÛÜ" +
            " àáâãäåæçèéêë" +
            " ìíîïñòóôõöøùúûüÿ"),
    ];
}
