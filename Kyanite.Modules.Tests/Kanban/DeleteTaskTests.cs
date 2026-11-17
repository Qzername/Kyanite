using Kyanite.Modules.Default.Kanban;

namespace Kyanite.Modules.Tests;

public class DeleteTaskTests
{
    [Fact]
    public void DeleteTask_TaskInColumn_RemovesTask()
    {
        var board = KanbanBoard.Create("a", "b");
        var item = new Item { Name = "task" };
        board.Columns[1].Items.Add(item);

        board.DeleteTaskCommand.Execute(item);

        Assert.DoesNotContain(item, board.Columns[0].Items);
        Assert.DoesNotContain(item, board.Columns[1].Items);
    }

    [Fact]
    public void DeleteTask_OtherTasksInColumn_LeavesThem()
    {
        var board = KanbanBoard.Create("a");
        var item = new Item { Name = "one" };
        var other = new Item { Name = "two" };
        board.Columns[0].Items.Add(item);
        board.Columns[0].Items.Add(other);

        board.DeleteTaskCommand.Execute(item);

        Assert.Equal([other], board.Columns[0].Items);
    }
}
