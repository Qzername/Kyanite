using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Dynamic;

namespace Kyanite.Modules.Text;

public partial class TextModule : ReactiveObject
{
    [Reactive]
    [Synchronize("text", "### Here you can write your text.")]
    public partial string Text { get; set; }
}
