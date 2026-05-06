using ReactiveUI.SourceGenerators;
using System;

namespace Kyanite.Modules.Text;

public partial class TextModule(Guid id) : Module(id)
{
    [Reactive]
    [Synchronize(nameof(Text), "### Here you can write your text.")]
    public partial string Text { get; set; }
}
