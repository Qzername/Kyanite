using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.Database.Default.GoogleDrive;

public partial class GoogleDriveInitializationViewModel : DatabaseInitializationViewModelBase
{
    [ObservableProperty] string _link = string.Empty;
}
