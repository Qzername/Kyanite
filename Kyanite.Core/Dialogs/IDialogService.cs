using System.ComponentModel;

namespace Kyanite.Core.Dialogs;

public interface IDialogService : INotifyPropertyChanged
{
    Dialog? CurrentDialog { get; }

    void Show(Dialog dialog);
    void Close();
}