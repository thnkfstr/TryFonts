using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using TryFonts.Core.Models;

namespace TryFonts.App.Converters;

/// <summary>Combines a variant's rendering metadata with the preview toggles.</summary>
public sealed class FontPreviewConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 0 || values[0] is not FontFamilyInfo font)
            return AvaloniaProperty.UnsetValue;

        var enabled = values.Count > 1 && values[1] is true;
        return parameter switch
        {
            "Weight" => (FontWeight)font.PreviewWeight(enabled),
            "Style" => font.PreviewItalic(enabled) ? FontStyle.Italic : FontStyle.Normal,
            "Stretch" => (FontStretch)font.Width,
            _ => AvaloniaProperty.UnsetValue,
        };
    }
}
