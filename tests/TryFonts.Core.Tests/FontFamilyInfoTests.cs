using TryFonts.Core.Models;

namespace TryFonts.Core.Tests;

public sealed class FontFamilyInfoTests
{
    [Fact]
    public void HasStyle_ReturnsTrueForAvailableStyle()
    {
        var styles = new HashSet<FontFaceStyle> { FontFaceStyle.Regular, FontFaceStyle.Bold };
        var font = new FontFamilyInfo("Arial", styles);

        Assert.True(font.HasStyle(FontFaceStyle.Regular));
        Assert.True(font.HasStyle(FontFaceStyle.Bold));
    }

    [Fact]
    public void HasStyle_ReturnsFalseForMissingStyle()
    {
        var styles = new HashSet<FontFaceStyle> { FontFaceStyle.Regular };
        var font = new FontFamilyInfo("Arial", styles);

        Assert.False(font.HasStyle(FontFaceStyle.Bold));
        Assert.False(font.HasStyle(FontFaceStyle.Italic));
        Assert.False(font.HasStyle(FontFaceStyle.BoldItalic));
    }

    [Fact]
    public void Record_Equality_BasedOnValues()
    {
        var styles = new HashSet<FontFaceStyle> { FontFaceStyle.Regular };
        var a = new FontFamilyInfo("Arial", styles);
        var b = new FontFamilyInfo("Arial", styles);

        // Records use structural equality by default
        Assert.Equal(a, b);
    }

    [Fact]
    public void SourcePath_IsOptional()
    {
        var font = new FontFamilyInfo("Arial",
            new HashSet<FontFaceStyle> { FontFaceStyle.Regular });

        Assert.Null(font.SourcePath);
    }

    [Fact]
    public void SourcePath_CanBeSet()
    {
        var font = new FontFamilyInfo("Arial",
            new HashSet<FontFaceStyle> { FontFaceStyle.Regular },
            SourcePath: "/Fonts/arial.ttf");

        Assert.Equal("/Fonts/arial.ttf", font.SourcePath);
    }
}
