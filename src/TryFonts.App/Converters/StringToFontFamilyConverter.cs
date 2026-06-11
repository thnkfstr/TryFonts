using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TryFonts.App.Converters;

/// <summary>
/// Converts a font family name string to an Avalonia <see cref="FontFamily"/>.
/// <para>
/// Avalonia's binding engine does not automatically apply the XAML TypeConverter
/// for <see cref="FontFamily"/>, so this explicit converter is required when
/// binding a string property to a FontFamily dependency property.
/// </para>
/// </summary>
public sealed class StringToFontFamilyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string name && !string.IsNullOrWhiteSpace(name))
            return new FontFamily(name);
        return FontFamily.Default;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is FontFamily ff ? ff.Name : string.Empty;
}
