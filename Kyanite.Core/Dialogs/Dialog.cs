using Kyanite.ViewModels;

namespace Kyanite.Core.Dialogs;

public class Dialog(string title, int width, int height, ViewModelBase viewModel) : ViewModelBase
{
    public readonly string Title = title;
    public int Width { get; } = width;
    public int Height { get; } = height;
    public ButtonInfo[] Buttons { get; set; } = [];
    public ViewModelBase ViewModel { get; } = viewModel;
}
