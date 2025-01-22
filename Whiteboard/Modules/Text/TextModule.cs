using Microsoft.Toolkit.Uwp.Notifications;
using ReactiveUI;

namespace Whiteboard.Modules.Text
{
    public class TextModule : Module
    {
        [Synchronize("text", "sample text")] string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _ = SendChangesToServer("text", _text);
                OnPropertyChanged("Text");
                this.RaiseAndSetIfChanged(ref _text, value, nameof(Text));
            }
        }


        public void Send()
        {
        }
    }
}
