using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.Database;

public abstract class DatabaseInitializationViewModelBase : ObservableObject
{
    public abstract Dictionary<string, string> GetData();
}