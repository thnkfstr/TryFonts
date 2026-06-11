using TryFonts.Core.Models;

namespace TryFonts.Core.Tests;

public sealed class SettingsTests
{
    // ── Default values ────────────────────────────────────────────────────────

    [Fact]
    public void Defaults_FontSize_Is24()
    {
        var s = new AppSettings();
        Assert.Equal(24.0, s.FontSize);
    }

    [Fact]
    public void Defaults_BoldAndItalic_AreFalse()
    {
        var s = new AppSettings();
        Assert.False(s.IsBold);
        Assert.False(s.IsItalic);
    }

    [Fact]
    public void Defaults_SearchMode_IsContains()
    {
        var s = new AppSettings();
        Assert.Equal(SearchMode.Contains, s.SearchMode);
    }

    [Fact]
    public void Defaults_SortMode_IsNameAZ()
    {
        var s = new AppSettings();
        Assert.Equal(SortMode.NameAZ, s.SortMode);
    }

    [Fact]
    public void Defaults_WindowGeometry_HasSensibleSize()
    {
        var s = new AppSettings();
        Assert.True(s.WindowWidth > 0);
        Assert.True(s.WindowHeight > 0);
    }

    [Fact]
    public void Defaults_WindowPosition_IsNaN()
    {
        var s = new AppSettings();
        // NaN signals "let the OS decide"
        Assert.True(double.IsNaN(s.WindowX));
        Assert.True(double.IsNaN(s.WindowY));
    }

    // ── Mutation ──────────────────────────────────────────────────────────────

    [Fact]
    public void Settings_CanBeModified()
    {
        var s = new AppSettings
        {
            FontSize   = 36,
            IsBold     = true,
            IsItalic   = true,
            SearchMode = SearchMode.StartsWith,
            SortMode   = SortMode.NameZA,
        };

        Assert.Equal(36,                 s.FontSize);
        Assert.True(s.IsBold);
        Assert.True(s.IsItalic);
        Assert.Equal(SearchMode.StartsWith, s.SearchMode);
        Assert.Equal(SortMode.NameZA,       s.SortMode);
    }

    // ── No PreviewText field ──────────────────────────────────────────────────

    [Fact]
    public void Settings_DoesNotPersistPreviewText()
    {
        // The spec says preview text must NEVER be persisted.
        // Confirm the property does not exist on the model.
        var type = typeof(AppSettings);
        Assert.Null(type.GetProperty("PreviewText"));
        Assert.Null(type.GetProperty("CustomText"));
        Assert.Null(type.GetProperty("LastPreviewText"));
    }
}
