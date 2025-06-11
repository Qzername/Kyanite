using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using Whiteboard.ViewModels;

namespace Whiteboard.ViewLocators;

//viewlocator used for default view searching for example in content control
internal class AvaloniaViewLocator : IDataTemplate
{
    public Control Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}
