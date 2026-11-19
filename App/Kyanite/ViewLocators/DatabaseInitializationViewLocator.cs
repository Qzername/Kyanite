using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Kyanite.Database;
using System;

namespace Kyanite.ViewLocators;

public class DatabaseInitializationViewLocator : IDataTemplate
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
            Text = "Database initialization view not found: " + name
        };
    }

    public bool Match(object? data) => data is DatabaseInitializationViewModelBase;
}