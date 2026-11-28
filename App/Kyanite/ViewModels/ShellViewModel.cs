using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Controls.DialogSystem;
using Kyanite.Core.Dialogs;
using Kyanite.Core.Services;
using Kyanite.Database;
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
	[ObservableProperty] DialogSystemViewModel dialogSystemViewModel;

	readonly IServiceProvider _serviceProvider;
	readonly IAppSettingsService _appSettingsService;

	public ShellViewModel(IServiceProvider serviceProvider, IAppSettingsService appSettingsService, IDialogService dialogService)
	{
		_serviceProvider = serviceProvider;
		_appSettingsService = appSettingsService;

		DialogSystemViewModel = new(dialogService);

		UpdateView();
	}

	public void UpdateView()
	{
		if (_appSettingsService.CurrentAppSettings.DatabaseInformationInitialized)
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
