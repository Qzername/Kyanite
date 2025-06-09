using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Whiteboard.Modules.Text
{
    public class TextModule : Module
    {
        [Synchronize("text", "sample text")]
        [Reactive]
        public string Text { get; set; }

        public void Send()
        {
        }
    }
}
