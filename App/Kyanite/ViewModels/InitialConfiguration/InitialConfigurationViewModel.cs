using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Controls.ErrorInformation;
using Kyanite.Core.Dialogs;
using Kyanite.Database;
using Kyanite.Exceptions;
using Kyanite.Services;
using Kyanite.ViewModels.InitialConfiguration.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Kyanite.ViewModels.InitialConfiguration;

internal partial class InitialConfigurationViewModel : ViewModelBase
{
    public enum Pages
    {
        PickStack,
        ProvidePrepareData,
        Finished
    }

    [ObservableProperty] int _page;
    [ObservableProperty] ObservableObject? _currentPageViewModel;

    readonly IServiceProvider _serviceProvider;
    readonly ShellViewModel _shellViewModel;
    readonly AppSettingsService _appSettingsService;
    readonly DatabaseStackProvider _databaseStackProvider;
    readonly PickDatabaseStackViewModel _pickDatabaseStackViewModel;
    readonly FinishedPageViewModel _finishedPageViewModel = new();

    DatabaseInitializationViewModelBase? _initializationViewModel;

    public InitialConfigurationViewModel(
        IServiceProvider serviceProvider,
        ShellViewModel shellViewModel,
        AppSettingsService appSettingsService,
        DatabaseStackLoader databaseStackLoader,
        DatabaseStackProvider databaseStackProvider)
    {
        _serviceProvider = serviceProvider;
        _shellViewModel = shellViewModel;
        _appSettingsService = appSettingsService;
        _databaseStackProvider = databaseStackProvider;

        _pickDatabaseStackViewModel = new PickDatabaseStackViewModel(serviceProvider, databaseStackLoader);
        CurrentPageViewModel = _pickDatabaseStackViewModel;
    }

    [RelayCommand]
    async Task GoToNextPage()
    {
        try
        {
            switch ((Pages)Page)
            {
                case Pages.PickStack:
                    if (string.IsNullOrWhiteSpace(_pickDatabaseStackViewModel.PickedDatabaseStack))
                    {
                        ShowError("Pick a database stack first.");
                        return;
                    }

                    _initializationViewModel = _pickDatabaseStackViewModel
                        .GetPickedStack()
                        .CreateInitializationViewModel();

                    CurrentPageViewModel = _initializationViewModel;
                    Page = (int)Pages.ProvidePrepareData;
                    break;

                case Pages.ProvidePrepareData:
                    await FinishInitializationAsync();
                    break;

                case Pages.Finished:
                    _shellViewModel.UpdateView();
                    break;
            }
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    [RelayCommand]
    void GoToPreviousPage()
    {
        if (Page <= (int)Pages.PickStack)
            return;

        Page--;

        CurrentPageViewModel = (Pages)Page switch
        {
            Pages.PickStack => _pickDatabaseStackViewModel,
            Pages.ProvidePrepareData => _initializationViewModel,
            _ => CurrentPageViewModel
        };
    }

    async Task FinishInitializationAsync()
    {
        if (_initializationViewModel is null)
            throw new InvalidOperationException("Database initialization data is missing.");

        if (_appSettingsService.CurrentAppSettings is null)
            throw new AppSettingsNotInitializedException();

        var currentStack = _pickDatabaseStackViewModel.GetPickedStack();
        var stackData = _initializationViewModel.GetData();

        if (!currentStack.IsPrepared && !await currentStack.Prepare(stackData))
            throw new InvalidOperationException("The selected database stack could not be prepared.");

        _databaseStackProvider.SetStack(currentStack);

        _appSettingsService.Save(_appSettingsService.CurrentAppSettings with
        {
            DatabaseInformation = stackData,
            DatabaseStackType = currentStack.GetType().Name,
            DatabaseinformationInitialized = true
        });

        CurrentPageViewModel = _finishedPageViewModel;
        Page = (int)Pages.Finished;
    }

    void ShowError(string message)
    {
        _serviceProvider.GetRequiredService<IDialogService>()
            .CreateBuilder()
            .WithTitle("Initialization error")
            .WithSize(480, 240)
            .WithViewModel(new ErrorInformationViewModel(message))
            .AddCloseButton()
            .BuildAndShow();
    }
}
