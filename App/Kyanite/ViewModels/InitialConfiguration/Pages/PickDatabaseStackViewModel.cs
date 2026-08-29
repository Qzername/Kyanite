using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Database;
using Kyanite.Database.Default.GoogleDrive;
using Kyanite.Database.Default.Local;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyanite.ViewModels.InitialConfiguration.Pages;

internal partial class PickDatabaseStackViewModel : ViewModelBase
{
    readonly DatabaseStackLoader _databaseStackLoader;

    //todo: make it versitile 
    public string[] AvailableStacks => [..loadedDatabaseStacks.Select(x => x.FriendlyName)];
    [ObservableProperty] string _pickedDatabaseStack = string.Empty;

    DatabaseStack[] loadedDatabaseStacks;

    public PickDatabaseStackViewModel(DatabaseStackLoader databaseStackLoader)
    {
        _databaseStackLoader = databaseStackLoader;

        LoadStacks();
    }

    void LoadStacks()
    {
        List<DatabaseStack> stacks = new();

        foreach (var stackNameAndType in _databaseStackLoader.DatabaseStackTypes)
        {
            var instance = Activator.CreateInstance(stackNameAndType.Value) ?? throw new Exception("DatabaseStack must contain parameterless constructor");
            stacks.Add((DatabaseStack)instance);
        }

        loadedDatabaseStacks = [.. stacks];
    }

    public DatabaseStack GetPickedStack()
        => loadedDatabaseStacks.Single(x => x.FriendlyName == PickedDatabaseStack);
}