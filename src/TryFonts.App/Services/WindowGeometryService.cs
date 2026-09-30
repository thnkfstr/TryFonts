using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using TryFonts.Core.Models;
using TryFonts.Core.Services;

namespace TryFonts.App.Services;

/// <summary>Restores visible window bounds and persists normal bounds separately from maximization.</summary>
public static class WindowGeometryService
{
    public static void Attach(Window window, ISettingsService settingsService)
    {
        var settings = settingsService.Load();
        var normal = new WindowGeometry(settings.WindowWidth, settings.WindowHeight,
            settings.WindowX, settings.WindowY);
        var maximized = settings.WindowMaximized;
        var ready = false;
        var closed = false;
        normal = Restore(window, normal);

        window.Opened += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            if (closed) return;
            // Initial platform sizing can move the frame. Restore position after
            // that sizing/layout pass, using the actual title bar and border size.
            normal = Restore(window, normal);
            ready = true;
            if (maximized)
                window.WindowState = WindowState.Maximized;
            else
                CaptureNormal();
        }, DispatcherPriority.Background);

        window.Resized += (_, _) => QueueCapture();
        window.PositionChanged += (_, _) => QueueCapture();
        window.PropertyChanged += (_, args) =>
        {
            if (ready && args.Property == Window.WindowStateProperty &&
                window.WindowState != WindowState.Minimized)
                maximized = window.WindowState == WindowState.Maximized;
        };
        window.Closing += (_, _) =>
        {
            CaptureNormal();
            var current = settingsService.Load();
            current.WindowWidth = normal.Width;
            current.WindowHeight = normal.Height;
            current.WindowX = normal.X;
            current.WindowY = normal.Y;
            current.WindowMaximized = maximized;
            settingsService.Save(current);
        };
        window.Closed += (_, _) => closed = true;

        void QueueCapture() => Dispatcher.UIThread.Post(CaptureNormal, DispatcherPriority.Background);

        void CaptureNormal()
        {
            if (!ready || closed || window.WindowState != WindowState.Normal)
                return;

            var size = window.ClientSize;
            if (size.Width > 0 && size.Height > 0)
                normal = new WindowGeometry(size.Width, size.Height, window.Position.X, window.Position.Y);
        }
    }

    private static WindowGeometry Restore(Window window, WindowGeometry geometry)
    {
        var hasPosition = double.IsFinite(geometry.X) && double.IsFinite(geometry.Y) &&
            geometry.X >= int.MinValue && geometry.X <= int.MaxValue &&
            geometry.Y >= int.MinValue && geometry.Y <= int.MaxValue;
        var screen = hasPosition
            ? window.Screens.ScreenFromPoint(new PixelPoint((int)geometry.X, (int)geometry.Y))
            : null;
        screen ??= window.Screens.Primary ?? window.Screens.All.FirstOrDefault();
        if (screen is null)
            return geometry;

        var client = window.ClientSize;
        var frame = window.FrameSize ?? client;
        var area = screen.WorkingArea;
        var constrained = geometry.ConstrainTo(area.X, area.Y, area.Width, area.Height,
            screen.Scaling, Math.Max(0, frame.Width - client.Width), Math.Max(0, frame.Height - client.Height));
        window.MinWidth = Math.Min(window.MinWidth, constrained.Width);
        window.MinHeight = Math.Min(window.MinHeight, constrained.Height);
        window.Width = constrained.Width;
        window.Height = constrained.Height;
        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Position = new PixelPoint((int)Math.Round(constrained.X), (int)Math.Round(constrained.Y));
        return constrained;
    }
}
