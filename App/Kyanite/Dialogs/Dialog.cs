using Kyanite.ViewModels;
using System;

namespace Kyanite.Dialogs;

internal class Dialog(string title, int width, int height, ViewModelBase viewModel) : ViewModelBase
{
    public readonly string Title = title;
    public readonly int Width = width, Height = height;
    public Action<Dialog>? OnClose;
    public ViewModelBase ViewModel { get; } = viewModel;
}
