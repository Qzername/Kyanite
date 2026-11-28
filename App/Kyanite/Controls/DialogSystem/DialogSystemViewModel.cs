using CommunityToolkit.Mvvm.Input;
using Kyanite.Core.Dialogs;
using Kyanite.ViewModels;

namespace Kyanite.Controls.DialogSystem;

/// <summary>
/// Control which manages dialog visuals
/// </summary>
internal partial class DialogSystemViewModel : ViewModelBase
{
	public IDialogService DialogService { get; }

	internal DialogSystemViewModel(IDialogService dialogService)
	{
		DialogService = dialogService;
	}

	[RelayCommand]
	void CloseDialog()
	{
		DialogService.Close();
	}
}
