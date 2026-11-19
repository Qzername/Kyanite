using Avalonia.Controls;
using Avalonia.Input;

namespace Kyanite.Modules.Default.Kanban;

public partial class KanbanView : UserControl
{
    private static readonly DataFormat<Item> ItemFormat = DataFormat.CreateInProcessFormat<Item>("kyanite-kanban-item");

    public KanbanView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private async void Task_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control || control.DataContext is not Item item)
            return;

        var data = new DataTransfer();
        data.Add(DataTransferItem.Create(ItemFormat, item));

        await DragDrop.DoDragDropAsync(e, data, DragDropEffects.Move);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer.TryGetValue(ItemFormat) is not null)
            e.DragEffects = DragDropEffects.Move;
        else
            e.DragEffects = DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        var item = e.DataTransfer.TryGetValue(ItemFormat);

        if (item is null)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        if (e.Source is not Control source)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        var targetColumn = FindColumn(source);

        if (targetColumn is null)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        if (DataContext is not KanbanViewModel viewModel)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        foreach (var column in viewModel.Columns)
            if (column.Items.Remove(item))
                break;

        targetColumn.Items.Add(item);
        e.DragEffects = DragDropEffects.Move;
    }

    private static ColumnViewModel? FindColumn(Control control)
    {
        Control? current = control;

        while (current is not null)
        {
            if (current.DataContext is ColumnViewModel column)
                return column;

            current = current.Parent as Control;
        }

        return null;
    }
}