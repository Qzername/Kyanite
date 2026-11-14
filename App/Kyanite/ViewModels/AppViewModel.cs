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

        desktop.MainWindow.Closed += async (_, _) =>
        {
            //async void: any exception thrown here (failed save, offline upload) would abort the process and lose the session
            try
            {
                var moduleManager = serviceProvider.GetRequiredService<ModuleManager>();
                var shellViewModelProvider = serviceProvider.GetRequiredService<ShellViewModelProvider>();
                var databaseStackProvider = serviceProvider.GetRequiredService<DatabaseStackProvider>();

                //app is not configured; close the app
                if (databaseStackProvider.ActiveStack is null)
                {
                    desktop.Shutdown();
                    return;
                }

                if (shellViewModelProvider.TryRetriveShellCurrentViewModel(out MainViewModel mainViewModel) &&
                    mainViewModel.SelectedModule is not null)
                    await moduleManager.SaveModule(mainViewModel.SelectedModule);

                await databaseStackProvider.ActiveStack.OnWindowClosing();

                moduleManager.ClearData();
                shellProvider.DisposeShell();
                desktop.MainWindow = null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Closing failed, window reopened: {ex}");

                //window is already gone; reopen it so the user sees the error instead of a vanished app
                desktop.MainWindow = null;
                OpenApplication();

                serviceProvider.GetRequiredService<IDialogService>()
                    .CreateBuilder()
                    .WithViewModel(new ErrorInformationViewModel(ex.Message))
                    .AddCloseButton()
                    .BuildAndShow();
            }
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

        DatabaseStackProvider databaseStackProvider = serviceProvider.GetRequiredService<DatabaseStackProvider>();

        if (databaseStackProvider.ActiveStack is not null)
            await databaseStackProvider.ActiveStack.OnWindowClosing();

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}