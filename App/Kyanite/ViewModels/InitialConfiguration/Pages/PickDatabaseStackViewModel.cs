using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyanite.ViewModels.InitialConfiguration.Pages;

internal partial class PickDatabaseStackViewModel : ViewModelBase
{
    readonly DatabaseStackLoader _databaseStackLoader;
    readonly IServiceProvider _serviceProvider;

    public IReadOnlyList<string> AvailableStacks => loadedDatabaseStacks.Select(stack => stack.FriendlyName).ToArray();

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
            var instance = ActivatorUtilities.CreateInstance(_serviceProvider, stackNameAndType.Value)
                ?? throw new InvalidOperationException($"Could not create database stack '{stackNameAndType.Key}'.");

            stacks.Add((DatabaseStack)instance);
        }

        loadedDatabaseStacks = [.. stacks];

        if (loadedDatabaseStacks.Length > 0 && string.IsNullOrEmpty(PickedDatabaseStack))
            PickedDatabaseStack = loadedDatabaseStacks[0].FriendlyName;
    }

    public DatabaseStack GetPickedStack()
        => loadedDatabaseStacks.SingleOrDefault(stack => stack.FriendlyName == PickedDatabaseStack)
           ?? throw new InvalidOperationException("Pick a valid database stack first.");
}
