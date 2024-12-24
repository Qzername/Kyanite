using ReactiveUI;
using System.Diagnostics;

namespace Whiteboard.Modules.Text
{
    public class TextModule : Module
    {
        [Synchronize("text")] string _text;
        public string Text
        {
            get => _text;
            set
            { 
                _text = value;
                _ = SendChangesToServer("text", _text);
                OnPropertyChanged("Text");
            }
        }
    }
}
