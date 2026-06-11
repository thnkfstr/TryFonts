using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TryFonts.App.ViewModels;

namespace TryFonts.App;

public partial class MainWindow : Window
{
    private ListBox? _fontListBox;

    // Scroll-anchoring state captured just before a font-size change.
    // The anchor is the topmost visible row, identified by ITEM INDEX (stable
    // across a resize, unlike pixel offsets or realized containers).
    private int    _anchorIndex = -1;
    private double _anchorFraction;        // how far the anchor row was scrolled past the top, 0..1
    private int    _restoreGeneration;     // invalidates an in-flight restore when a new resize arrives
    private bool   _restoreInProgress;

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
                    CaptureAnchor();
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
    /// Captures the anchor just before a font-size change, while layout is still intact:
    /// the ITEM INDEX of the topmost visible row, plus how far that row was scrolled
    /// past the viewport top (as a fraction of its height, so it survives the resize).
    ///
    /// Realized containers are scanned by actual viewport position (TranslatePoint),
    /// not collection order — with virtualization, panel.Children order is realization
    /// order and does not match visual order.
    ///
    /// If a restore is already in flight (held repeat button), the existing anchor is
    /// kept: the viewport is mid-correction and re-capturing would anchor to the wrong row.
    /// </summary>
    private void CaptureAnchor()
    {
        if (_restoreInProgress) return;

        _anchorIndex    = -1;
        _anchorFraction = 0;

        var scrollViewer = _fontListBox?.FindDescendantOfType<ScrollViewer>();
        if (_fontListBox is null || scrollViewer is null) return;

        Control? topmost  = null;
        var      topmostY = double.MaxValue;

        foreach (var container in _fontListBox.GetRealizedContainers())
        {
            var pt = container.TranslatePoint(new Point(0, 0), scrollViewer);
            if (!pt.HasValue) continue;

            var top    = pt.Value.Y;
            var bottom = top + container.Bounds.Height;

            if (bottom <= 0.5) continue;            // fully above the viewport
            if (top < topmostY)
            {
                topmostY = top;
                topmost  = container;
            }
        }

        if (topmost is null) return;

        _anchorIndex = _fontListBox.IndexFromContainer(topmost);
        if (topmostY < 0 && topmost.Bounds.Height > 0)
            _anchorFraction = Math.Min(1, -topmostY / topmost.Bounds.Height);
    }

    /// <summary>
    /// Pins the anchor row back to the viewport top after the font-size layout pass.
    ///
    /// Pixel-offset math cannot work here: row heights do not scale linearly with font
    /// size (fixed padding, fixed-size meta column, text wrapping changes line counts),
    /// and the panel's extent is an estimate while items are unrealized. Instead this
    /// converges iteratively: realize the anchor row (ScrollIntoView), measure its exact
    /// viewport position (TranslatePoint), correct the offset, repeat. Each correction
    /// realizes rows nearer the target, so the estimate error shrinks every pass;
    /// it typically settles in 1–2 iterations, 8 is a safety cap.
    ///
    /// A generation counter abandons this restore if another font-size change arrives
    /// (held repeat button) — the newest restore wins, reusing the same anchor.
    /// </summary>
    private async Task RestoreScrollAsync()
    {
        if (_fontListBox is null || _anchorIndex < 0) return;

        var scrollViewer = _fontListBox.FindDescendantOfType<ScrollViewer>();
        if (scrollViewer is null) return;

        var generation = ++_restoreGeneration;
        var index      = _anchorIndex;
        var fraction   = _anchorFraction;

        _restoreInProgress = true;
        try
        {
            for (var attempt = 0; attempt < 8; attempt++)
            {
                // Let the pending layout pass (font-size change or our last offset
                // correction) complete before measuring.
                await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
                if (generation != _restoreGeneration) return; // superseded by a newer resize

                var container = _fontListBox.ContainerFromIndex(index);
                if (container is null)
                {
                    // Anchor row not realized — bring it into the realized window and retry.
                    _fontListBox.ScrollIntoView(index);
                    continue;
                }

                var pt = container.TranslatePoint(new Point(0, 0), scrollViewer);
                if (!pt.HasValue)
                {
                    _fontListBox.ScrollIntoView(index);
                    continue;
                }

                // Target viewport-space Y of the row's top edge: scrolled past the top
                // by the same fraction of its (new) height as before the resize.
                var desiredTop = -fraction * container.Bounds.Height;
                var error      = pt.Value.Y - desiredTop;
                if (Math.Abs(error) < 0.5) return;   // pinned

                var maxY = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
                scrollViewer.Offset = new Vector(
                    scrollViewer.Offset.X,
                    Math.Clamp(scrollViewer.Offset.Y + error, 0, maxY));
            }
        }
        finally
        {
            if (generation == _restoreGeneration)
                _restoreInProgress = false;
        }
    }

    private void SetupKeyboardShortcuts()
    {
        // Platform command modifier: Cmd on macOS, Ctrl elsewhere.
        var isMac = OperatingSystem.IsMacOS();
        var cmd   = isMac ? KeyModifiers.Meta : KeyModifiers.Control;

        // Keep tooltips truthful per platform (XAML defaults say "Ctrl+…")
        if (this.FindControl<TextBox>("PreviewTextBox") is { } previewBox)
            ToolTip.SetTip(previewBox, isMac ? "⌘L to focus" : "Ctrl+L to focus");
        if (this.FindControl<TextBox>("SearchBox") is { } searchBox)
            ToolTip.SetTip(searchBox, isMac ? "/ or ⌘F to focus, Esc to clear"
                                            : "/ or Ctrl+F to focus, Esc to clear");

        KeyDown += (_, e) =>
        {
            if (e.Key == Key.OemQuestion && e.KeyModifiers == KeyModifiers.None)
            {
                FocusControl("SearchBox");
                e.Handled = true;
            }
            else if (e.Key == Key.F && e.KeyModifiers == cmd)
            {
                FocusControl("SearchBox");
                e.Handled = true;
            }
            else if (e.Key == Key.L && e.KeyModifiers == cmd)
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
