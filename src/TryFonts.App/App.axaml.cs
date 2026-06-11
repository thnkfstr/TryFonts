using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TryFonts.App.Services;
using TryFonts.App.ViewModels;

namespace TryFonts.App;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var settingsService = new JsonSettingsService();
            var fontService = new SkiaFontDiscoveryService();
            var settings = settingsService.Load();

            var vm = new MainWindowViewModel(
                fontService,
                settingsService,
                syntheticFontCount: AppStartupArgs.SyntheticFontCount);

            var window = new MainWindow { DataContext = vm };

            // Restore window geometry
            window.Width  = double.IsNaN(settings.WindowWidth)  ? 1200 : settings.WindowWidth;
            window.Height = double.IsNaN(settings.WindowHeight) ? 800  : settings.WindowHeight;

            if (!double.IsNaN(settings.WindowX) && !double.IsNaN(settings.WindowY))
            {
                window.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.Manual;
                window.Position = new PixelPoint(
                    (int)settings.WindowX,
                    (int)settings.WindowY);
            }

            // Save geometry when the window closes
            window.Closing += (_, _) =>
            {
                vm.SaveWindowGeometry(
                    window.Width,
                    window.Height,
                    window.Position.X,
                    window.Position.Y);
            };

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
