using ReactiveUI.Fody.Helpers;

namespace Whiteboard.Modules.Text
{
    public class TextModule : Module
    {
        [Reactive]
        bool editMode { get; set; } = false;

        [Reactive]
        [Synchronize("text", "sample text")]
        public string Text { get; set; }

        public void SwitchEditMode() => editMode = !editMode;
    }
}
