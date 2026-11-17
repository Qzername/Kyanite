using System.Text.Json;
using Kyanite.Modules.Default.Kanban;

namespace Kyanite.Modules.Tests;

public class KanbanUiStateTests
{
    [Fact]
    public void Serialize_ColumnWithDragAndDeleteState_OmitsUiState()
    {
        var board = KanbanBoard.Create("a");
        var column = board.Columns[0];
        column.Opacity = 0.4;
        column.IsPendingDelete = true;

        // same call ModuleManager.CreateDataInformation makes for a [Synchronize] field
        var json = JsonSerializer.Serialize(board.Columns, board.Columns.GetType());

        Assert.DoesNotContain("Opacity", json);
        Assert.DoesNotContain("IsPendingDelete", json);
        Assert.DoesNotContain("DeleteButtonText", json);
        Assert.Contains("\"Name\":\"a\"", json);
    }
}
