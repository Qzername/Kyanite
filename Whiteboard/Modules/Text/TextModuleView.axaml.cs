using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Whiteboard.Modules.Text;

public partial class TextModuleView : ReactiveUserControl<TextModule>
{
    public TextModuleView()
    {
        InitializeComponent();
    }
}