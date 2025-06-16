using System;
using Whiteboard.ViewModels;

namespace Whiteboard.Services;

internal class PopupService 
{
    PopupViewModel? _popupViewModel;

    public event Action OnPopupClosed; // Initialize with an empty delegate to avoid nullability issues  

    public void SetPopupHandler(PopupViewModel popupViewModel)
    {
        _popupViewModel = popupViewModel;
    }

    public void ShowPopup(ViewModelBase popup)
    {
        if (_popupViewModel is null)
            throw new Exception("PopupViewModel is not initialized.");

        _popupViewModel.Open(popup);
    }

    public void ClosePopup()
    {
        if (_popupViewModel is null)
            throw new Exception("PopupViewModel is not initialized.");

        _popupViewModel.Close();
        OnPopupClosed?.Invoke();
    }
}
