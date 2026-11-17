using Kyanite.Modules.Default.Kanban;

namespace Kyanite.Modules.Tests;

public class MoveColumnTests
{
    [Fact]
    public void MoveColumn_InsertBefore_MovesColumnBeforeTarget()
    {
        var board = KanbanBoard.Create("a", "b", "c");

        board.MoveColumn(board.Columns[2], board.Columns[0], insertBefore: true);

        Assert.Equal("c,a,b", KanbanBoard.Order(board));
    }

    [Fact]
    public void MoveColumn_InsertAfter_MovesColumnAfterTarget()
    {
        var board = KanbanBoard.Create("a", "b", "c");

        board.MoveColumn(board.Columns[0], board.Columns[2], insertBefore: false);

        Assert.Equal("b,c,a", KanbanBoard.Order(board));
    }

    [Fact]
    public void MoveColumn_BetweenColumns_InsertsBetweenThem()
    {
        var board = KanbanBoard.Create("a", "b", "c");

        board.MoveColumn(board.Columns[0], board.Columns[1], insertBefore: false);

        Assert.Equal("b,a,c", KanbanBoard.Order(board));
    }

    [Fact]
    public void MoveColumn_TargetNotInBoard_LeavesColumnsUnchanged()
    {
        var board = KanbanBoard.Create("a", "b", "c");

        board.MoveColumn(board.Columns[0], new ColumnViewModel { Name = "gone" }, insertBefore: true);

        Assert.Equal("a,b,c", KanbanBoard.Order(board));
    }

    [Fact]
    public void MoveColumn_TargetIsSource_LeavesColumnsUnchanged()
    {
        var board = KanbanBoard.Create("a", "b", "c");

        board.MoveColumn(board.Columns[1], board.Columns[1], insertBefore: true);

        Assert.Equal("a,b,c", KanbanBoard.Order(board));
    }
}
