using Avalonia.Controls;
using System;
using Whiteboard.ViewModels;

namespace Whiteboard.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closed += OnClosed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OnClose();
    }
}
