
using Kyanite.ViewModels;

namespace Kyanite.Core.Dialogs;

public class DialogBuilder(IDialogService dialogService)
{
    readonly IDialogService _dialogService = dialogService;

    string title = "Dialog";
    int width = 300, height = 200;

    ViewModelBase? viewModel;

    List<RawButtonInfo> buttons = [];

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
        buttons.Add(new RawButtonInfo()
        {
            Content = content,
            Command = onPress,
            AutoClose = closeWhenPressed
        });
        return this;
    }

    public Dialog Build()
    {
        if (viewModel is null)
            throw new InvalidOperationException("ViewModel must be set before building the dialog.");

        var dialog = new Dialog(title, width, height, viewModel);


        if (buttons.Count == 0)
        {
            dialog.Buttons = [
                new ButtonInfo(){
                    Command = _dialogService.Close,
                    Content = "Close"
                }
            ];
        }
        else
            dialog.Buttons = buttons.Select(x => new ButtonInfo()
            {
                Content = x.Content,
                Command = () =>
                {
                    x.Command(dialog);

                    if (x.AutoClose)
                        _dialogService.Close();
                }
            }).ToArray();

        return dialog;
    }

    public Dialog BuildAndShow()
    {
        var dialog = Build();
        _dialogService.Show(dialog);
        return dialog;
    }
}
