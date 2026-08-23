
using Kyanite.ViewModels;
using System;

namespace Kyanite.Dialogs;

internal class DialogBuilder
{
    string title = "Dialog";
    int width = 300, height = 200;

    ViewModelBase? viewModel;
    Action<Dialog>? onCloseAction;

    public DialogBuilder WithTitle(string title)
    {
        this.title = title;
        return this;
    }

    public DialogBuilder WithSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        return this;
    }

    public DialogBuilder WithViewModel(ViewModelBase viewModel)
    {
        this.viewModel = viewModel;
        return this;
    }

    public DialogBuilder SetOnClose(Action<Dialog> onCloseAction)
    {
        this.onCloseAction = onCloseAction;
        return this;
    }

    public Dialog Build()
    {
        if (viewModel is null)
            throw new InvalidOperationException("ViewModel must be set before building the dialog.");

        var dialog = new Dialog(title, width, height, viewModel)
        {
            OnClose = onCloseAction
        };

        return dialog;
    }
}
