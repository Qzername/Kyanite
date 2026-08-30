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
    INotificationService? notificationService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        var serviceProvider = collection.BuildServiceProvider();

        if (notificationService is not null)
        {
            var notificationServiceProvider = serviceProvider.GetRequiredService<NotificationServiceProvider>();
            notificationServiceProvider.SetService(notificationService);
        }

        DataContext = serviceProvider.GetRequiredService<AppViewModel>();
        var shellViewModel = serviceProvider.GetRequiredService<ShellViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = shellViewModel
            };

            desktop.MainWindow.Closing += async (sender, e) =>
            {
                var databaseStackProvider = serviceProvider.GetRequiredService<DatabaseStackProvider>();

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

    public void RegisterNotificationService(INotificationService notificationService)
    {
        this.notificationService = notificationService;
    }
}