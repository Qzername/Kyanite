using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Kyanite.Modules;
using System;

namespace Kyanite.ViewLocators;

public class ModuleViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        string? fullName = param.GetType().FullName;

        if (string.IsNullOrEmpty(fullName))
            return null;

        var name = fullName.Replace("ViewModel", "View", StringComparison.Ordinal);

        var assembly = param.GetType().Assembly;
        var type = assembly.GetType(name);

        if (type is not null)
            return (Control)Activator.CreateInstance(type)!;

        return new TextBlock
        {
            Text = "Module not found: " + name
        };
    }

    public bool Match(object? data) => data is Module;
}