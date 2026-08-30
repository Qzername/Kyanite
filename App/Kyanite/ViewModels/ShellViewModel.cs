using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Database;
using Kyanite.Dialogs;
using Kyanite.Exceptions;
using Kyanite.Services;
using Kyanite.ViewModels.InitialConfiguration;
using Kyanite.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kyanite.ViewModels;

/// <summary>
/// controls what content is now display, main page or database initalizaiton page
/// </summary>
internal partial class ShellViewModel : ViewModelBase
{
    [ObservableProperty] ViewModelBase? _currentViewModel;

    public DialogService DialogService { get; }
    readonly IServiceProvider _serviceProvider;
    readonly AppSettingsService _appSettingsService;

    public ShellViewModel(IServiceProvider serviceProvider, AppSettingsService appSettingsService, DialogService dialogService)
    {
        _serviceProvider = serviceProvider;
        _appSettingsService = appSettingsService;
        DialogService = dialogService;

        UpdateView();
    }

    [RelayCommand]
    void CloseDialog()
    {
        DialogService.Close();
    }

    public void UpdateView()
    {
        if (_appSettingsService.CurrentAppSettings is null)
            throw new AppSettingsNotInitializedException();

        if (_appSettingsService.CurrentAppSettings.DatabaseinformationInitialized)
            CurrentViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        else
            CurrentViewModel = new InitialConfigurationViewModel(
                _serviceProvider,
                shellViewModel: this,
                _appSettingsService,
                _serviceProvider.GetRequiredService<DatabaseStackLoader>(),
                _serviceProvider.GetRequiredService<DatabaseStackProvider>()
            );
    }
}
