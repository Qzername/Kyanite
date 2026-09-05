namespace Kyanite.Modules.Default.ToDoList;

internal class ToDoElement
{
    public required string Name { get; init; }
    public bool IsCompleted { get; set; } = false;
}