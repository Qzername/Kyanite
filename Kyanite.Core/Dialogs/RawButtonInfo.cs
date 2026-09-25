namespace Kyanite.Core.Dialogs;

public struct RawButtonInfo
{
    public string Content { get; set; }
    public Action<Dialog> Command { get; set; }
    public bool AutoClose { get; set; }
}

public struct ButtonInfo
{
    public string Content { get; set; }
    public Action Command { get; set; }
}