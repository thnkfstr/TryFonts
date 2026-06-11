using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TryFonts.App.ViewModels;
using TryFonts.Core.Models;

namespace TryFonts.App;

public partial class MainWindow : Window
{
    private ListBox? _fontListBox;

    // Scroll-preservation state captured just before a font-size change
    private string _topFamilyBeforeResize  = string.Empty;
    private double _scrollOffsetBeforeResize;
    private double _fontSizeBeforeResize;

    public MainWindow()
    {
        InitializeComponent();
        SetupKeyboardShortcuts();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainWindowViewModel vm)
        {
            _fontListBox = this.FindControl<ListBox>("FontListBox");

            // Capture top item just before FontSize changes (layout still intact)
            vm.PropertyChanging += (_, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.FontSize))
                    CaptureTopFamily();
            };

            // After FontSize changes, restore the top-visible item exactly.
            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.FontSize))
                    _ = RestoreScrollAsync();
            };
        }
    }

    /// <summary>
    /// Captures scroll state before a font-size change while layout is still intact:
    /// the family name of the top-visible row, the raw scroll offset, and the current
    /// font size. All three are needed by RestoreScrollAsync.
    /// </summary>
    private void CaptureTopFamily()
    {
        _topFamilyBeforeResize   = string.Empty;
        _scrollOffsetBeforeResize = 0;
        _fontSizeBeforeResize    = 0;

        var scrollViewer = _fontListBox?.FindDescendantOfType<ScrollViewer>();
        var panel        = _fontListBox?.FindDescendantOfType<VirtualizingStackPanel>();
        if (scrollViewer is null || panel is null) return;

        // Record offset and font size BEFORE the change (PropertyChanging fires first)
        _scrollOffsetBeforeResize = scrollViewer.Offset.Y;
        if (DataContext is MainWindowViewModel vm)
            _fontSizeBeforeResize = vm.FontSize; // still the old value here

        var offsetY = scrollViewer.Offset.Y;
        foreach (var child in panel.Children)
        {
            if (child is Control ctrl &&
                ctrl.Bounds.Bottom > offsetY &&
                ctrl.DataContext is FontFamilyInfo info)
            {
                _topFamilyBeforeResize = info.FamilyName;
                return;
            }
        }
    }

    /// <summary>
    /// Two-path scroll restoration — no ScrollIntoView calls.
    ///
    /// ScrollIntoView uses VirtualizingStackPanel's average-height estimate to locate
    /// unrealized items, which can be badly wrong and is the root cause of random jumps.
    /// This method avoids it entirely.
    ///
    /// Primary path: after one Background-priority yield (layout settled), scan the
    /// realized children.  If the target is still in the viewport (common for small
    /// increments), TranslatePoint gives its exact position and we pin it to the top.
    ///
    /// Fallback path: item heights scale linearly with font size, so the scroll offset
    /// should scale by the same ratio.  newOffset = capturedOffset × (newSize / oldSize).
    /// Deterministic, no estimation, no random behavior.
    /// </summary>
    private async Task RestoreScrollAsync()
    {
        if (_fontListBox is null) return;

        var vm = DataContext as MainWindowViewModel;
        if (vm is null) return;

        // Snapshot and clear captured state
        var targetName     = _topFamilyBeforeResize;
        var capturedOffset = _scrollOffsetBeforeResize;
        var capturedSize   = _fontSizeBeforeResize;
        _topFamilyBeforeResize = string.Empty;

        var scrollViewer = _fontListBox.FindDescendantOfType<ScrollViewer>();
        var panel        = _fontListBox.FindDescendantOfType<VirtualizingStackPanel>();
        if (scrollViewer is null || panel is null) return;

        // Wait for the font-size layout pass to complete
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

        // ── Primary: exact pin via TranslatePoint ────────────────────────────────
        if (!string.IsNullOrEmpty(targetName))
        {
            foreach (var child in panel.Children)
            {
                if (child is not Control ctrl) continue;
                if (ctrl.DataContext is not FontFamilyInfo info) continue;
                if (info.FamilyName != targetName) continue;

                // viewport-space Y of the item's top edge (0 = viewport top)
                var pt = ctrl.TranslatePoint(new Point(0, 0), scrollViewer);
                if (pt.HasValue)
                {
                    var maxY = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
                    scrollViewer.Offset = new Vector(
                        scrollViewer.Offset.X,
                        Math.Clamp(scrollViewer.Offset.Y + pt.Value.Y, 0, maxY));
                    return;
                }
            }
        }

        // ── Fallback: proportional scaling ───────────────────────────────────────
        // Item not realized in the current viewport. All item heights scale with font size,
        // so the scroll offset should scale by the same factor.
        // Computed entirely from pre-change captures — no post-change estimation needed.
        if (capturedSize > 0)
        {
            var scale = vm.FontSize / capturedSize;
            var newY  = capturedOffset * scale;
            var maxY  = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
            scrollViewer.Offset = new Vector(scrollViewer.Offset.X, Math.Clamp(newY, 0, maxY));
        }
    }

    private void SetupKeyboardShortcuts()
    {
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.OemQuestion && e.KeyModifiers == KeyModifiers.None)
            {
                FocusControl("SearchBox");
                e.Handled = true;
            }
            else if (e.Key == Key.F && e.KeyModifiers == KeyModifiers.Control)
            {
                FocusControl("SearchBox");
                e.Handled = true;
            }
            else if (e.Key == Key.L && e.KeyModifiers == KeyModifiers.Control)
            {
                FocusControl("PreviewTextBox");
                e.Handled = true;
            }
        };
    }

    private void FocusControl(string name)
    {
        if (this.FindControl<Control>(name) is { } control)
        {
            control.Focus();
            if (control is TextBox tb)
                tb.SelectAll();
        }
    }
}
