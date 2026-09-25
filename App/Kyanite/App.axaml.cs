using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kyanite.Services;
using Kyanite.ViewModels;
using Kyanite.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

public partial class App : Application
{
    NotificationServiceProvider _notificationServiceProvider = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection
            .AddSingleton(_notificationServiceProvider)
            .AddCommonServices();

        var serviceProvider = collection.BuildServiceProvider();

        _notificationServiceProvider = serviceProvider.GetRequiredService<NotificationServiceProvider>();

        var appViewModel = serviceProvider.GetRequiredService<AppViewModel>();
        DataContext = appViewModel;

        var shellViewModel = serviceProvider.GetRequiredService<ShellViewModelProvider>().CreateOrGetShell();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            appViewModel.OpenApplication();
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
        _notificationServiceProvider.SetService(notificationService);
    }
}