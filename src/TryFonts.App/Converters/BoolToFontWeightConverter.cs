using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TryFonts.App.Converters;

/// <summary>Converts a <see cref="bool"/> to <see cref="FontWeight"/>.</summary>
/// <remarks>
/// <c>true</c> → <see cref="FontWeight.Bold"/>;
/// <c>false</c> → <see cref="FontWeight.Normal"/>
/// </remarks>
public sealed class BoolToFontWeightConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? FontWeight.Bold : FontWeight.Normal;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is FontWeight fw && fw == FontWeight.Bold;
}
