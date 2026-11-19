using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.ViewModels;

namespace Kyanite.Controls.ErrorInformation;

internal partial class ErrorInformationViewModel(string errorMessage) : ViewModelBase
{
    [ObservableProperty] string _errorMessage = errorMessage;
}
