using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TryFonts.App.Converters;

/// <summary>Converts a <see cref="bool"/> to <see cref="FontStyle"/>.</summary>
/// <remarks>
/// <c>true</c> → <see cref="FontStyle.Italic"/>;
/// <c>false</c> → <see cref="FontStyle.Normal"/>
/// </remarks>
public sealed class BoolToFontStyleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? FontStyle.Italic : FontStyle.Normal;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is FontStyle fs && fs == FontStyle.Italic;
}
