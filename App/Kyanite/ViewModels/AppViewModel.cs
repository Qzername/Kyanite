using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Database;
using Kyanite.Modules;
using Kyanite.Services;
using Kyanite.ViewModels.Main;
using Kyanite.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Kyanite.ViewModels;

/// <summary>
/// viewmodel for entire application
/// </summary>
internal partial class AppViewModel(IServiceProvider serviceProvider) : ViewModelBase
{
    [RelayCommand]
    public void OpenApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;

        if (desktop.MainWindow is not null)
            return;

        var shellProvider = serviceProvider.GetRequiredService<ShellViewModelProvider>();

        desktop.MainWindow = new MainWindow()
        {
            DataContext = shellProvider.CreateOrGetShell()
        };

        desktop.MainWindow.Closed += async (sender, e) =>
        {
            var databaseStackProvider = serviceProvider.GetRequiredService<DatabaseStackProvider>();

            //app is not configured; close the app
            if (databaseStackProvider.ActiveStack is null)
            {
                desktop.Shutdown();
                return;
            }

            await databaseStackProvider.ActiveStack.OnWindowClosing();

            shellProvider.DisposeShell();
            desktop.MainWindow = null;

            var moduleManager = serviceProvider.GetRequiredService<ModuleManager>();
            moduleManager.ClearData();
        };

        desktop.MainWindow.Show();
    }

    [RelayCommand]
    async Task CloseApplication()
    {
        //save current module if any
        var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

        if (mainViewModel.SelectedModule is not null)
        {
            var moduleManager = serviceProvider.GetRequiredService<ModuleManager>();
            await moduleManager.SaveModule(mainViewModel.SelectedModule);
        }

        DatabaseStack databaseStack = serviceProvider.GetRequiredService<DatabaseStack>();
        await databaseStack.OnWindowClosing();

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}