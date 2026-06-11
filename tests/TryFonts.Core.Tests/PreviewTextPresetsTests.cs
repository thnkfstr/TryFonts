using TryFonts.Core.Services;

namespace TryFonts.Core.Tests;

public sealed class PreviewTextPresetsTests
{
    [Fact]
    public void BaseSampleText_IsNotEmpty()
    {
        Assert.NotEmpty(PreviewTextPresets.BaseSampleText);
    }

    [Fact]
    public void BaseSampleText_ContainsRequiredContent()
    {
        var text = PreviewTextPresets.BaseSampleText;

        // The spec mandates this exact string (modulo the & escaping in XML)
        Assert.Contains("quick brown fox", text);
        Assert.Contains("lazy dogs", text);
        Assert.Contains("2,345", text);
        Assert.Contains("$7", text);
        Assert.Contains("$0.89", text);
        Assert.Contains("#6", text);
    }

    [Fact]
    public void All_ContainsAtLeast6Presets()
    {
        Assert.True(PreviewTextPresets.All.Count >= 6,
            $"Expected at least 6 presets, got {PreviewTextPresets.All.Count}");
    }

    [Fact]
    public void All_FirstPresetIsBaseSample()
    {
        var first = PreviewTextPresets.All[0];
        Assert.Equal(PreviewTextPresets.BaseSampleText, first.Text);
    }

    [Fact]
    public void All_AllPresetsHaveNonEmptyNameAndText()
    {
        foreach (var preset in PreviewTextPresets.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(preset.Name),
                $"Preset has empty name");
            Assert.False(string.IsNullOrWhiteSpace(preset.Text),
                $"Preset '{preset.Name}' has empty text");
        }
    }

    [Fact]
    public void All_PresetNamesAreUnique()
    {
        var names = PreviewTextPresets.All.Select(p => p.Name).ToList();
        var distinct = names.Distinct().ToList();
        Assert.Equal(names.Count, distinct.Count);
    }

    /// <summary>
    /// Verifies the spec requirement: preview text must NOT be persisted.
    /// The ViewModel always starts with BaseSampleText, not user-typed text.
    /// This is a model-level guard — the AppSettings type must not have a
    /// PreviewText property.
    /// </summary>
    [Fact]
    public void AppSettings_DoesNotHavePreviewTextProperty()
    {
        var type = typeof(TryFonts.Core.Models.AppSettings);
        var prop = type.GetProperty("PreviewText");
        Assert.Null(prop);
    }
}
