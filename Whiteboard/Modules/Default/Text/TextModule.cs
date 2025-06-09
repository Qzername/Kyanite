using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Whiteboard.Modules.Text
{
    public class TextModule : Module
    {
        [Reactive]
        [Synchronize("text", "sample text")]
        public string Text { get; set; }
    }
}
