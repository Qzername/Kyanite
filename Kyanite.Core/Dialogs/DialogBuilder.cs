using Kyanite.ViewModels;

namespace Kyanite.Core.Dialogs;

public class DialogBuilder(IDialogService dialogService)
{
    readonly IDialogService _dialogService = dialogService;
    readonly List<RawButtonInfo> buttons = [];

    string title = "Dialog";
    int width = 300, height = 200;

    ViewModelBase? viewModel;

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

    public DialogBuilder AddButton(string content, Action<Dialog> onPress, bool closeWhenPressed = true)
    {
        buttons.Add(new RawButtonInfo(content, onPress, closeWhenPressed));
        return this;
    }

    public DialogBuilder AddCloseButton(string content = "Close")
    {
        buttons.Add(new RawButtonInfo(content, _ => { }, true));
        return this;
    }

    public Dialog Build()
    {
        if (viewModel is null)
            throw new InvalidOperationException("ViewModel must be set before building the dialog.");

        var dialog = new Dialog(title, width, height, viewModel);

        dialog.Buttons = [.. buttons.Select(button => new ButtonInfo(button.Content, () =>
        {
            button.Command(dialog);

            if (button.AutoClose)
                _dialogService.Close();
        }))];

        return dialog;
    }

    public Dialog BuildAndShow()
    {
        var dialog = Build();
        _dialogService.Show(dialog);
        return dialog;
    }
}
