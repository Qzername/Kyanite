using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Database;
using Kyanite.Exceptions;
using Kyanite.Services;
using Kyanite.ViewModels.InitialConfiguration.Pages;
using System;

namespace Kyanite.ViewModels.InitialConfiguration;

internal partial class InitialConfigurationViewModel : ViewModelBase
{
    public enum Pages
    {
        PickStack,
        ProvidePrepareData,
        Finitialization
    }

    const int TotalPages = 3;

    [ObservableProperty] int _page;
    [ObservableProperty] ObservableObject? _currentPageViewModel;

    readonly ShellViewModel _shellViewModel;
    readonly AppSettingsService _appSettingsService;
    readonly DatabaseStackProvider _databaseStackProvider;

    readonly PickDatabaseStackViewModel pickDatabaseStackViewModel;
    readonly FinishedPageViewModel finishedPageViewModel = new();

    public InitialConfigurationViewModel(ShellViewModel shellViewModel, AppSettingsService appSettingsService, DatabaseStackLoader databaseStackLoader, DatabaseStackProvider databaseStackProvider)
    {
        _shellViewModel = shellViewModel;
        _appSettingsService = appSettingsService;
        _databaseStackProvider = databaseStackProvider;

        pickDatabaseStackViewModel = new(databaseStackLoader);

        CurrentPageViewModel = pickDatabaseStackViewModel;
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
        Pages page = (Pages)value;

        switch(page)
        {
            case Pages.PickStack:
                CurrentPageViewModel = pickDatabaseStackViewModel;
                break;

            case Pages.ProvidePrepareData:
                var vm = pickDatabaseStackViewModel.GetPickedStack().InitializationViewModel;
                CurrentPageViewModel = vm;
                break;

            case Pages.Finitialization:
                var currentStack = pickDatabaseStackViewModel.GetPickedStack();

                currentStack.Prepare();
                _databaseStackProvider.SetStack(currentStack);

                CurrentPageViewModel = finishedPageViewModel;
                break;

            default: throw new Exception("Unexpected page value");
        }
    }
}
