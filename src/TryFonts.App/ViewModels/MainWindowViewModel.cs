using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    // ── Dependencies ────────────────────────────────────────────────────────

    private readonly IFontDiscoveryService _fontService;
    private readonly ISettingsService _settingsService;
    private readonly int _syntheticFontCount;

    // ── Internal state ───────────────────────────────────────────────────────

    private List<FontFamilyInfo> _allFonts = [];
    private CancellationTokenSource _debounceCts = new();

    // ── Observable properties ────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FontCountDisplay))]
    private IReadOnlyList<FontFamilyInfo> _filteredFonts = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FontCountDisplay))]
    private int _totalFontCount;

    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _statusMessage = "Discovering fonts…";

    // Controls — NOT persisted
    [ObservableProperty] private string _previewText = PreviewTextPresets.BaseSampleText;

    // Controls — persisted
    [ObservableProperty] private double _fontSize;
    [ObservableProperty] private bool _isBold;
    [ObservableProperty] private bool _isItalic;
    [ObservableProperty] private SearchMode _searchMode;
    [ObservableProperty] private SortMode _sortMode;

    // Search box
    [ObservableProperty] private string _searchText = string.Empty;

    // Preset selector
    [ObservableProperty] private PreviewTextPreset? _selectedPreset;

    // ── Static data exposed to the UI ────────────────────────────────────────

    public IReadOnlyList<PreviewTextPreset> Presets => PreviewTextPresets.All;

    public IReadOnlyList<string> SearchModeItems { get; } = ["Contains", "Starts with"];
    public IReadOnlyList<string> SortModeItems   { get; } = ["Name A–Z", "Name Z–A"];

    /// <summary>Index-based binding for ComboBox SelectedIndex.</summary>
    public int SearchModeIndex
    {
        get => (int)SearchMode;
        set
        {
            if ((int)SearchMode != value)
            {
                SearchMode = (SearchMode)value;
                OnPropertyChanged();
            }
        }
    }

    public int SortModeIndex
    {
        get => (int)SortMode;
        set
        {
            if ((int)SortMode != value)
            {
                SortMode = (SortMode)value;
                OnPropertyChanged();
            }
        }
    }

    public string FontCountDisplay =>
        $"{FilteredFonts.Count:N0} / {TotalFontCount:N0} fonts";

    // ── Constructor ───────────────────────────────────────────────────────────

    public MainWindowViewModel(
        IFontDiscoveryService fontService,
        ISettingsService settingsService,
        int syntheticFontCount = 0)
    {
        _fontService = fontService;
        _settingsService = settingsService;
        _syntheticFontCount = syntheticFontCount;

        // Restore persisted settings — but never restore preview text
        var s = _settingsService.Load();
        _fontSize   = s.FontSize;
        _isBold     = s.IsBold;
        _isItalic   = s.IsItalic;
        _searchMode = s.SearchMode;
        _sortMode   = s.SortMode;

        _ = LoadFontsAsync();
    }

    // ── Font loading ──────────────────────────────────────────────────────────

    private async Task LoadFontsAsync()
    {
        try
        {
            var discovered = await _fontService.DiscoverAsync();

            if (_syntheticFontCount > 0)
            {
                // Augment with synthetic entries; keep real names for rendering hints
                var realNames = discovered.Select(f => f.FamilyName).ToList();
                var synthetic = SyntheticFontDataGenerator.Generate(_syntheticFontCount, realNames);
                _allFonts = [.. discovered, .. synthetic];
            }
            else
            {
                _allFonts = [.. discovered];
            }

            TotalFontCount = _allFonts.Count;
            ApplyFilterAndSort();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Font discovery error: {ex.Message}";
            IsLoading = false;
            return;
        }

        IsLoading = false;
        StatusMessage = string.Empty;
    }

    // ── Reactive callbacks ────────────────────────────────────────────────────

    partial void OnSearchTextChanged(string value) => _ = DebounceFilterAsync();

    partial void OnSearchModeChanged(SearchMode value)
    {
        ApplyFilterAndSort();
        SaveSettings();
    }

    partial void OnSortModeChanged(SortMode value)
    {
        ApplyFilterAndSort();
        SaveSettings();
    }

    partial void OnFontSizeChanged(double value)     => SaveSettings();
    partial void OnIsBoldChanged(bool value)         => SaveSettings();
    partial void OnIsItalicChanged(bool value)       => SaveSettings();

    partial void OnSelectedPresetChanged(PreviewTextPreset? value)
    {
        if (value is not null)
            PreviewText = value.Text;
    }

    // ── Filtering ─────────────────────────────────────────────────────────────

    private async Task DebounceFilterAsync()
    {
        // Cancel any pending debounce
        _debounceCts.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        try
        {
            await Task.Delay(150, token);
            ApplyFilterAndSort();
        }
        catch (OperationCanceledException) { /* superseded by a newer keystroke */ }
    }

    private void ApplyFilterAndSort()
    {
        var filtered = FontFilter.Apply(_allFonts, SearchText, SearchMode);
        var sorted   = FontSorter.Apply(filtered, SortMode);
        FilteredFonts = sorted.ToList().AsReadOnly();
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    [RelayCommand]
    private void ClearSearch() => SearchText = string.Empty;

    [RelayCommand]
    private void ResetPreviewText() => PreviewText = PreviewTextPresets.BaseSampleText;

    // ── Settings ──────────────────────────────────────────────────────────────

    private void SaveSettings()
    {
        _settingsService.Save(new AppSettings
        {
            FontSize   = FontSize,
            IsBold     = IsBold,
            IsItalic   = IsItalic,
            SearchMode = SearchMode,
            SortMode   = SortMode,
        });
    }

    /// <summary>Called by App.axaml.cs when the main window closes.</summary>
    public void SaveWindowGeometry(double width, double height, double x, double y)
    {
        var s = _settingsService.Load();
        s.WindowWidth  = width;
        s.WindowHeight = height;
        s.WindowX      = x;
        s.WindowY      = y;
        _settingsService.Save(s);
    }
}
