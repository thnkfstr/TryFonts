using System.Globalization;
using Avalonia.Data.Converters;
using TryFonts.Core.Models;

namespace TryFonts.App.Converters;

/// <summary>
/// Converts an <see cref="IReadOnlySet{FontFaceStyle}"/> to an opacity double.
/// Returns 1.0 when the style named by <c>ConverterParameter</c> is present,
/// 0.25 when it is absent.
/// </summary>
/// <example>
/// <code>
/// Opacity="{Binding AvailableStyles,
///           Converter={StaticResource StyleAvailabilityConverter},
///           ConverterParameter=Bold}"
/// </code>
/// </example>
public sealed class StyleAvailabilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IReadOnlySet<FontFaceStyle> styles &&
            parameter is string paramStr &&
            Enum.TryParse<FontFaceStyle>(paramStr, ignoreCase: true, out var style))
        {
            return styles.Contains(style) ? 1.0 : 0.25;
        }

        return 0.25;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
