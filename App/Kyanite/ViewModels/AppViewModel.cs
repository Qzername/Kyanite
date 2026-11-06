using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Controls.ErrorInformation;
using Kyanite.Core.Dialogs;
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
/// View model for the entire application.
/// </summary>
internal partial class AppViewModel(IServiceProvider serviceProvider) : ViewModelBase
{
    bool _isShuttingDown;

    [RelayCommand]
    public void OpenApplication()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        if (desktop.MainWindow is not null)
        {
            desktop.MainWindow.Show();
            desktop.MainWindow.Activate();
            return;
        }

        var shellProvider = serviceProvider.GetRequiredService<ShellViewModelProvider>();

        var window = new MainWindow
        {
            DataContext = shellProvider.CreateOrGetShell()
        };

        desktop.MainWindow = window;
        window.Closed += async (_, _) => await OnMainWindowClosedAsync(desktop, shellProvider);
        window.Show();
    }

    [RelayCommand]
    async Task CloseApplication()
    {
        try
        {
            _isShuttingDown = true;

            await SaveCurrentModuleAsync();

            var databaseStackProvider = serviceProvider.GetRequiredService<DatabaseStackProvider>();

            if (databaseStackProvider.ActiveStack is not null)
                await databaseStackProvider.ActiveStack.OnWindowClosing();

            serviceProvider.GetRequiredService<ModuleManager>().ClearData();
            serviceProvider.GetRequiredService<ShellViewModelProvider>().DisposeShell();

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = null;
                desktop.Shutdown();
            }
        }
        catch (Exception exception)
        {
            _isShuttingDown = false;
            ShowError(exception.Message);
        }
    }

    async Task OnMainWindowClosedAsync(
        IClassicDesktopStyleApplicationLifetime desktop,
        ShellViewModelProvider shellProvider)
    {
        if (_isShuttingDown)
            return;

        try
        {
            var databaseStackProvider = serviceProvider.GetRequiredService<DatabaseStackProvider>();

            // The app is not configured yet, so close it instead of leaving it in the tray.
            if (databaseStackProvider.ActiveStack is null)
            {
                desktop.Shutdown();
                return;
            }

            await SaveCurrentModuleAsync();
            await databaseStackProvider.ActiveStack.OnWindowClosing();
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
        finally
        {
            serviceProvider.GetRequiredService<ModuleManager>().ClearData();
            shellProvider.DisposeShell();
            desktop.MainWindow = null;
        }
    }

    async Task SaveCurrentModuleAsync()
    {
        var shellProvider = serviceProvider.GetRequiredService<ShellViewModelProvider>();

        if (!shellProvider.TryRetriveShellCurrentViewModel(out MainViewModel mainViewModel) ||
            mainViewModel.SelectedModule is null)
        {
            return;
        }

        await serviceProvider.GetRequiredService<ModuleManager>().SaveModule(mainViewModel.SelectedModule);
    }

    void ShowError(string message)
    {
        serviceProvider.GetRequiredService<IDialogService>()
            .CreateBuilder()
            .WithTitle("Error")
            .WithSize(460, 240)
            .WithViewModel(new ErrorInformationViewModel(message))
            .AddCloseButton()
            .BuildAndShow();
    }
}
