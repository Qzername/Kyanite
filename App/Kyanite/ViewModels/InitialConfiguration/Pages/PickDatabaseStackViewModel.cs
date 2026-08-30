using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Database;
using Kyanite.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyanite.ViewModels.InitialConfiguration.Pages;

internal partial class PickDatabaseStackViewModel : ViewModelBase
{
    readonly DatabaseStackLoader _databaseStackLoader;
    readonly IServiceProvider _serviceProvider;

    //todo: make it versitile 
    public string[] AvailableStacks => [.. loadedDatabaseStacks.Select(x => x.FriendlyName)];
    [ObservableProperty] string _pickedDatabaseStack = string.Empty;

    DatabaseStack[] loadedDatabaseStacks = [];

    public PickDatabaseStackViewModel(IServiceProvider serviceProvider, DatabaseStackLoader databaseStackLoader)
    {
        _databaseStackLoader = databaseStackLoader;
        _serviceProvider = serviceProvider;

        LoadStacks();
    }

    void LoadStacks()
    {
        List<DatabaseStack> stacks = [];

        foreach (var stackNameAndType in _databaseStackLoader.DatabaseStackTypes)
        {
            var instance = ActivatorUtilities.CreateInstance(_serviceProvider, stackNameAndType.Value) ?? throw new MissingParameterlessConstructorException();
            stacks.Add((DatabaseStack)instance);
        }

        loadedDatabaseStacks = [.. stacks];
    }

    public DatabaseStack GetPickedStack()
        => loadedDatabaseStacks.Single(x => x.FriendlyName == PickedDatabaseStack);
}