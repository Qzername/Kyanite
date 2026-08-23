using System.ComponentModel;

namespace Kyanite.Dialogs;

internal class DialogService : INotifyPropertyChanged
{
    private Dialog? _currentDialog;

    public Dialog? CurrentDialog
    {
        get => _currentDialog;
        private set
        {
            if (_currentDialog == value)
                return;

            _currentDialog = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDialog)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Show(Dialog dialog)
    {
        CurrentDialog = dialog;
    }

    public void Close()
    {
        CurrentDialog?.OnClose?.Invoke(CurrentDialog);
        CurrentDialog = null;
    }
}