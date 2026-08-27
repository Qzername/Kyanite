using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Exceptions;
using Kyanite.Services;
using Kyanite.ViewModels.InitialConfiguration.Pages;
using System;

namespace Kyanite.ViewModels.InitialConfiguration;

internal partial class InitialConfigurationViewModel : ViewModelBase
{
    const int TotalPages = 3;

    [ObservableProperty] int _page;
    [ObservableProperty] ViewModelBase? _currentPageViewModel;

    readonly ShellViewModel _shellViewModel;
    readonly AppSettingsService _appSettingsService;

    public InitialConfigurationViewModel(ShellViewModel shellViewModel, AppSettingsService appSettingsService)
    {
        _shellViewModel = shellViewModel;
        _appSettingsService = appSettingsService;

        CurrentPageViewModel = new PickDatabaseStackViewModel();
    }

    [RelayCommand]
    void GoToNextPage()
    {
        if (Page + 1 >= TotalPages)
        {
            if (_appSettingsService.CurrentAppSettings is null)
                throw new AppSettingsNotInitialized();

            _appSettingsService.Save(_appSettingsService.CurrentAppSettings with
            {
                DatabaseinformationInitialized = true
            });

            _shellViewModel.UpdateView();

            return;
        }

        Page++;
    }

    [RelayCommand]
    void GoToPreviousPage()
    {
        if (Page - 1 < 0)
            return;

        Page--;
    }

    partial void OnPageChanged(int value)
    {
        CurrentPageViewModel = value switch
        {
            0 => new PickDatabaseStackViewModel(),
            1 => null,
            2 => new FinishedPageViewModel(),
            _ => throw new Exception("Unexpected page value")
        };
    }
}
