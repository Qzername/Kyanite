using Kyanite.Database;
using Kyanite.Modules.Default.Kanban;

namespace Kyanite.Modules.Tests;

static class KanbanBoard
{
    public static KanbanViewModel Create(params string[] names)
    {
        var board = new KanbanViewModel(new ModuleInformation { Name = "board", Type = "Kanban" });

        foreach (var name in names)
            board.Columns.Add(new ColumnViewModel { Name = name });

        return board;
    }

    public static string Order(KanbanViewModel board) => string.Join(",", board.Columns.Select(column => column.Name));
}
