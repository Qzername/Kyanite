using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Modules;
using Kyanite.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Kyanite.ViewModels;

internal partial class AppViewModel(IServiceProvider serviceProvider) : ViewModelBase
{
    [RelayCommand]
    void OpenApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;

        if (desktop.MainWindow!.IsVisible)
            return;

        desktop.MainWindow = new MainWindow()
        {
            DataContext = serviceProvider.GetRequiredService<MainViewModel>()
        };
        desktop.MainWindow.Show();
    }

    [RelayCommand]
    async Task CloseApplication()
    {
        //save current module if any
        var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
        
        if(mainViewModel.SelectedModule is not null)
        {
            var moduleManager = serviceProvider.GetRequiredService<ModuleManager>();
            await moduleManager.SaveModule(mainViewModel.SelectedModule);
        }

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}