using Kyanite.ViewModels;

namespace Kyanite.Core.Dialogs;

public class Dialog : ViewModelBase
{
	internal Dialog(string title, DialogSize dialogSize, ViewModelBase viewModel)
	{
		Title = title;
		DialogSize = dialogSize;
		ViewModel = viewModel;
	}

	public readonly string Title;
	public DialogSize DialogSize;
	public ButtonInfo[] Buttons { get; set; } = [];
	public ViewModelBase ViewModel { get; }
}
