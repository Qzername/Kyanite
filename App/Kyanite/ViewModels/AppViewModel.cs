using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

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
    void CloseApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        desktop.Shutdown();
    }
}