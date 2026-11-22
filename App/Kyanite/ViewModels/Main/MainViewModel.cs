using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Dialogs.AppSettings;
using Kyanite.Controls.ErrorInformation;
using Kyanite.Controls.ModulePicker;
using Kyanite.Core.Dialogs;
using Kyanite.Modules;
using Kyanite.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Kyanite.Core.Services;

namespace Kyanite.ViewModels.Main;

internal partial class MainViewModel : ViewModelBase
{
	readonly IServiceProvider _serviceProvider;
	readonly ModuleManager _moduleManager;
	readonly IDialogService _dialogService;
	readonly DatabaseStackProvider _databaseStackProvider;
	readonly IAppSettingsService _appSettingsService;

	[ObservableProperty] bool _isDataLoaded;
	[ObservableProperty] bool _isPaneOpen = true;

	public ObservableCollection<Module> AllModules { get; } = [];
	[ObservableProperty] Module? _selectedModule;

	public MainViewModel(IServiceProvider serviceProvider, ModuleManager moduleManager, IDialogService dialogService, DatabaseStackProvider databaseStackProvider, IAppSettingsService appSettingsService)
	{
		AllModules.Clear();

		_serviceProvider = serviceProvider;
		_moduleManager = moduleManager;
		_dialogService = dialogService;
		_databaseStackProvider = databaseStackProvider;
		_appSettingsService = appSettingsService;

		_ = PrepareModuleManager();
	}

	[RelayCommand]
	async Task ManuallySynchronizeDatabase()
	{
		if (_databaseStackProvider.ActiveStack is null)
			return;

		await _databaseStackProvider.ActiveStack.OnWindowClosing();
	}

	[RelayCommand]
	void OpenSettings()
	{
		_dialogService.CreateBuilder()
			.WithTitle("Settings")
			.WithSize(800, 600)
			.WithViewModel(new AppSettingsViewModel(_appSettingsService))
			.AddButton("Reset", _ => ResetInitialization())
			.AddButton("Save", OnSettingsDialogClosed)
			.BuildAndShow();
	}

	void OnSettingsDialogClosed(Dialog dialog)
	{
		if (dialog.ViewModel is not AppSettingsViewModel vm)
			return;

		var appSettings = _appSettingsService.CurrentAppSettings;

		//Notification defaults
		appSettings.DueSoonThresholdMinutes = vm.DueSoonThresholdMinutes;
		appSettings.EnableNotifications = vm.EnableNotifications;
		appSettings.NotifyOnOverdue = vm.NotifyOnOverdue;

		//Run in background setting
		appSettings.KeepRunningInBackground = vm.KeepRunningInBackground;

		_appSettingsService.Save();
	}

	[RelayCommand]
	void CreateModule()
	{
		// module picker throws when the manager is still loading; a click during startup must not kill the app
		if (!_moduleManager.IsStackPrepared)
			return;

		_dialogService.CreateBuilder()
			.WithTitle("Create dialog")
			.WithSize(600, 400)
			.WithViewModel(new ModulePickerViewModel(_moduleManager))
			.AddButton("Close",
			(dialog) =>
			{
				var modulePickerVM = (ModulePickerViewModel)dialog.ViewModel;
				_ = FinalizeCreateModule(modulePickerVM.ModuleName, modulePickerVM.SelectedModule);
			})
			.BuildAndShow();
	}

	void ResetInitialization()
	{
		var appSettingsService = _serviceProvider.GetRequiredService<IAppSettingsService>();

		var appSettings = appSettingsService.CurrentAppSettings;

		appSettings.DatabaseInformationInitialized = false;
		appSettings.DatabaseStackType = null;
		appSettings.DatabaseInformation = [];

		appSettingsService.Save();

		_databaseStackProvider.ActiveStack?.OnRemoved();
		AllModules.Clear();
		_moduleManager.ClearData();

		var shellViewModel = _serviceProvider.GetRequiredService<ShellViewModelProvider>().CreateOrGetShell();
		shellViewModel.UpdateView();
	}

	[RelayCommand]
	async Task DeleteModule(object moduleObj)
	{
		if (moduleObj is not Module module)
			throw new Exception("Provided object is not of a module type");

		await _moduleManager.DeleteModule(module);
		AllModules.Replace(_moduleManager.Modules);
	}

	[RelayCommand] void TogglePane() => IsPaneOpen = !IsPaneOpen;


	partial void OnSelectedModuleChanging(Module? oldValue, Module? newValue)
	{
		_ = SaveOldAndLoadNew(oldValue, newValue);
	}

	async Task FinalizeCreateModule(string moduleName, string selectedModule)
	{
		if (string.IsNullOrEmpty(selectedModule))
			return;

		var createdModule = await _moduleManager.CreateModule(moduleName, selectedModule);

		AllModules.Replace(_moduleManager.Modules);

		SelectedModule = createdModule;
	}

	async Task SaveOldAndLoadNew(Module? oldValue, Module? newValue)
	{
		if (oldValue is not null)
			await _moduleManager.SaveModule(oldValue);

		if (newValue is not null)
			await _moduleManager.LoadModule(newValue);
	}

	async Task PrepareModuleManager()
	{
		try
		{
			IAppSettingsService appSettingsService = _serviceProvider.GetRequiredService<IAppSettingsService>();

			if (_databaseStackProvider.ActiveStack is null)
				throw new Exception("Stackproviders Active stack is needed to be selected before module manager can be prepared");

			if (!_moduleManager.IsStackPrepared)
				await _moduleManager.Prepare(_serviceProvider, _databaseStackProvider.ActiveStack, appSettingsService.CurrentAppSettings.DatabaseInformation);

			AllModules.Replace(_moduleManager.Modules);

			IsDataLoaded = true;
		}
		catch (Exception ex)
		{
			_dialogService.CreateBuilder()
				.WithViewModel(new ErrorInformationViewModel(ex.Message))
				.AddCloseButton()
				.BuildAndShow();
		}
	}
}
