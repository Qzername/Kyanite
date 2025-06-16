using ReactiveUI.Fody.Helpers;
using Whiteboard.Services;

namespace Whiteboard.ViewModels;

public class PopupViewModel :ViewModelBase
{
    [Reactive] bool isOpen { get; set; } = false;
    [Reactive] ViewModelBase popupContentViewModel { get; set; }

    public PopupViewModel()
    {
        GetService<PopupService>().SetPopupHandler(this);
    }

    public void Open(ViewModelBase popup)
    {
        popupContentViewModel = popup;
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false; 
    }
}
