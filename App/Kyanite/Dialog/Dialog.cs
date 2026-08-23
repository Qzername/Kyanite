using Kyanite.ViewModels;

namespace Kyanite.Dialog;

internal class Dialog(string title, int width, int height) : ViewModelBase
{
    public readonly string Title = title;
    public readonly int Width = width, Height = height;
}
