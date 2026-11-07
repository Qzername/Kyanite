namespace Kyanite.Core.Dialogs;

public sealed record RawButtonInfo(string Content, Action<Dialog> Command, bool AutoClose = true);

public sealed record ButtonInfo(string Content, Action Command);
