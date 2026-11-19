using System;
using PleasantUI.Controls;

namespace Kyanite.Views;

public partial class MainWindow : PleasantWindow
{
    public MainWindow()
    {
        InitializeComponent();
        if (!OperatingSystem.IsWindows())
            EnableBlur = false;
    }
}