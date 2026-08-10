using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Kyanite.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    void Minimalize(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    void Toggle(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    void CloseWindow(object? sender, RoutedEventArgs e) => Close();

}