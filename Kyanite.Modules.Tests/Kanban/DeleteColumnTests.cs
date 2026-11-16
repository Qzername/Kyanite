using Kyanite.Modules.Default.Kanban;

namespace Kyanite.Modules.Tests;

public class DeleteColumnTests
{
    [Fact]
    public void FirstClickOnlyArms()
    {
        var board = KanbanBoard.Create("a");
        var column = board.Columns[0];

        board.RequestDeleteColumnCommand.Execute(column);

        Assert.Single(board.Columns);
        Assert.True(column.IsPendingDelete);
    }

    [Fact]
    public void SecondClickRemovesColumn()
    {
        var board = KanbanBoard.Create("a");
        var column = board.Columns[0];

        board.RequestDeleteColumnCommand.Execute(column);
        board.RequestDeleteColumnCommand.Execute(column);

        Assert.Empty(board.Columns);
    }

    [Fact]
    public void ArmingAnotherColumnDisarmsTheFirst()
    {
        var board = KanbanBoard.Create("a", "b");
        var first = board.Columns[0];
        var second = board.Columns[1];

        board.RequestDeleteColumnCommand.Execute(first);
        board.RequestDeleteColumnCommand.Execute(second);

        Assert.False(first.IsPendingDelete);
        Assert.True(second.IsPendingDelete);
        Assert.Equal(2, board.Columns.Count);
    }

    [Fact]
    public void CancelDisarms()
    {
        var board = KanbanBoard.Create("a");
        var column = board.Columns[0];

        board.RequestDeleteColumnCommand.Execute(column);
        board.CancelPendingDelete();

        Assert.False(column.IsPendingDelete);
        Assert.Single(board.Columns);

        board.RequestDeleteColumnCommand.Execute(column);

        Assert.Single(board.Columns);
    }
}
