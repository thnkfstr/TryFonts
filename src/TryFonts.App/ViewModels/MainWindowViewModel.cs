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

    // Guards against the FontSize ↔ FontSizeText circular update
    private bool _updatingFontSize;

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

    /// <summary>
    /// String representation of <see cref="FontSize"/> for the custom stepper TextBox.
    /// Kept in sync with <see cref="FontSize"/>; changes here parse back to <see cref="FontSize"/>.
    /// </summary>
    [ObservableProperty] private string _fontSizeText = "24";

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
        _fontSize      = s.FontSize;
        _fontSizeText  = ((int)Math.Round(s.FontSize)).ToString();
        _isBold        = s.IsBold;
        _isItalic      = s.IsItalic;
        _searchMode    = s.SearchMode;
        _sortMode      = s.SortMode;

        // Show the base-sample preset as active on startup
        _selectedPreset = PreviewTextPresets.All[0];

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

    partial void OnFontSizeChanged(double value)
    {
        // Sync the display text without re-triggering this method
        if (!_updatingFontSize)
        {
            _updatingFontSize = true;
            FontSizeText = ((int)Math.Round(value)).ToString();
            _updatingFontSize = false;
        }
        SaveSettings();
    }

    partial void OnFontSizeTextChanged(string value)
    {
        // Parse the user-typed value back to FontSize
        if (!_updatingFontSize &&
            int.TryParse(value.Trim(), out var size))
        {
            var clamped = (double)Math.Clamp(size, 6, 200);
            if (Math.Abs(FontSize - clamped) > 0.001)
            {
                _updatingFontSize = true;
                FontSize = clamped;
                _updatingFontSize = false;
            }
        }
    }

    partial void OnIsBoldChanged(bool value)   => SaveSettings();
    partial void OnIsItalicChanged(bool value) => SaveSettings();

    partial void OnSelectedPresetChanged(PreviewTextPreset? value)
    {
        if (value is not null)
            PreviewText = value.Text;
    }

    // ── Filtering ─────────────────────────────────────────────────────────────

    private async Task DebounceFilterAsync()
    {
        _debounceCts.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;
        try
        {
            await Task.Delay(150, token);
            ApplyFilterAndSort();
        }
        catch (OperationCanceledException) { }
    }

    private void ApplyFilterAndSort()
    {
        var filtered = FontFilter.Apply(_allFonts, SearchText, SearchMode);
        var sorted   = FontSorter.Apply(filtered, SortMode.NameAZ);
        FilteredFonts = sorted.ToList().AsReadOnly();
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    [RelayCommand]
    private void ClearSearch() => SearchText = string.Empty;

    [RelayCommand]
    private void IncreaseFontSize() => FontSize = Math.Min(FontSize + 2, 200);

    [RelayCommand]
    private void DecreaseFontSize() => FontSize = Math.Max(FontSize - 2, 6);

    // ── Settings ──────────────────────────────────────────────────────────────

    private void SaveSettings()
    {
        _settingsService.Save(new AppSettings
        {
            FontSize   = FontSize,
            IsBold     = IsBold,
            IsItalic   = IsItalic,
            SearchMode = SearchMode,
            SortMode   = SortMode.NameAZ,
        });
    }

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
