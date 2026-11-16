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

    public InitialConfigurationViewModel(IServiceProvider serviceProvider, ShellViewModel shellViewModel, AppSettingsService appSettingsService, DatabaseStackLoader databaseStackLoader, DatabaseStackProvider databaseStackProvider)
    {
        _shellViewModel = shellViewModel;
        _appSettingsService = appSettingsService;
        _databaseStackProvider = databaseStackProvider;

        pickDatabaseStackViewModel = new(serviceProvider, databaseStackLoader);

        CurrentPageViewModel = pickDatabaseStackViewModel;
    }

    [RelayCommand]
    void GoToNextPage()
    {
        if (Page + 1 >= TotalPages)
        {
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

        switch (page)
        {
            case Pages.PickStack:
                CurrentPageViewModel = pickDatabaseStackViewModel;
                break;

            case Pages.ProvidePrepareData:
                var vm = pickDatabaseStackViewModel.GetPickedStack().CreateInitializationViewModel();
                CurrentPageViewModel = vm;
                break;

            case Pages.Finitialization:
                var currentStack = pickDatabaseStackViewModel.GetPickedStack();

                if (CurrentPageViewModel is not DatabaseInitializationViewModelBase initVm)
                    throw new Exception("Current ViewModel expected to be of type: " + nameof(DatabaseInitializationViewModelBase));

                _databaseStackProvider.SetStack(currentStack);

                if (_appSettingsService.CurrentAppSettings is null)
                    throw new AppSettingsNotInitializedException();

                _appSettingsService.Save(_appSettingsService.CurrentAppSettings with
                {
                    DatabaseInformation = initVm.GetData(),
                    DatabaseStackType = currentStack.GetType().Name,
                    DatabaseinformationInitialized = true
                });

                CurrentPageViewModel = finishedPageViewModel;
                break;

            default: throw new Exception("Unexpected page value");
        }
    }
}
