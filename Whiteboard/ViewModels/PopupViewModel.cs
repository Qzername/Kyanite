using ReactiveUI.Fody.Helpers;

namespace Whiteboard.ViewModels;

public class PopupViewModel :ViewModelBase
{
    public static PopupViewModel Instance { get; private set; }

    [Reactive] bool isOpen { get; set; } = false;
    [Reactive] ViewModelBase popupContentViewModel { get; set; }

    public PopupViewModel()
    {
        Instance = this;
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
