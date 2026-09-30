namespace TryFonts.Core.Models;

/// <summary>Client size in logical units and frame position in screen pixels.</summary>
public sealed record WindowGeometry(double Width, double Height, double X, double Y)
{
    public WindowGeometry ConstrainTo(
        double areaX, double areaY, double areaWidth, double areaHeight,
        double scaling, double frameWidth, double frameHeight)
    {
        var maxWidth = Math.Max(1, areaWidth / scaling - frameWidth);
        var maxHeight = Math.Max(1, areaHeight / scaling - frameHeight);
        var width = Math.Clamp(double.IsFinite(Width) && Width > 0 ? Width : 1200,
            Math.Min(640, maxWidth), maxWidth);
        var height = Math.Clamp(double.IsFinite(Height) && Height > 0 ? Height : 800,
            Math.Min(400, maxHeight), maxHeight);
        var outerWidth = Math.Ceiling((width + frameWidth) * scaling);
        var outerHeight = Math.Ceiling((height + frameHeight) * scaling);
        var maxX = Math.Max(areaX, areaX + areaWidth - outerWidth);
        var maxY = Math.Max(areaY, areaY + areaHeight - outerHeight);

        return new WindowGeometry(width, height,
            double.IsFinite(X) ? Math.Clamp(X, areaX, maxX) : (areaX + maxX) / 2,
            double.IsFinite(Y) ? Math.Clamp(Y, areaY, maxY) : (areaY + maxY) / 2);
    }
}
