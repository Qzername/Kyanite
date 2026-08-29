using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kyanite.Exceptions;
using Kyanite.Services;
using Kyanite.ViewModels;
using Kyanite.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        var services = collection.BuildServiceProvider();

        DataContext = services.GetRequiredService<AppViewModel>();
        var shellViewModel = services.GetRequiredService<ShellViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = shellViewModel
            };

            desktop.MainWindow.Closing += async (sender, e) =>
            {
                var databaseStackProvider = services.GetRequiredService<DatabaseStackProvider>();

                if (databaseStackProvider.ActiveStack is null)
                    throw new ActiveStackNotInitializedExpection();

                await databaseStackProvider.ActiveStack.OnWindowClosing();
            };

            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new ShellView { DataContext = shellViewModel };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new ShellView
            {
                DataContext = shellViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}