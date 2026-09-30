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

            var vm = new MainWindowViewModel(
                fontService,
                settingsService,
                syntheticFontCount: AppStartupArgs.SyntheticFontCount);

            var window = new MainWindow { DataContext = vm };

            WindowGeometryService.Attach(window, settingsService);

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
