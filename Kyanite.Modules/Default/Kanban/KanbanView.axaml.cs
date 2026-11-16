using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Kyanite.Modules.Default.Kanban;

public partial class KanbanView : UserControl
{
    private static readonly DataFormat<Item> ItemFormat = DataFormat.CreateInProcessFormat<Item>("kyanite-kanban-item");
    private static readonly DataFormat<ColumnViewModel> ColumnFormat = DataFormat.CreateInProcessFormat<ColumnViewModel>("kyanite-kanban-column");

    public KanbanView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, UpdateDropTarget);
        AddHandler(DragDrop.DragEnterEvent, UpdateDropTarget);
        AddHandler(DragDrop.DropEvent, OnDrop);
        // DragOver/DragEnter fire only over columns, so leaving them must clear the marker
        AddHandler(DragDrop.DragLeaveEvent, (_, _) => ClearHighlight());
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        (DataContext as KanbanViewModel)?.CancelPendingDelete();
    }

    private async void Task_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control || control.DataContext is not Item item
            || !e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
            return;

        var data = new DataTransfer();
        data.Add(DataTransferItem.Create(ItemFormat, item));

        try
        {
            await DragDrop.DoDragDropAsync(e, data, DragDropEffects.Move);
        }
        finally
        {
            ClearHighlight();
        }
    }

    private async void Column_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control || control.DataContext is not ColumnViewModel column
            || !e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
            return;

        var data = new DataTransfer();
        data.Add(DataTransferItem.Create(ColumnFormat, column));

        column.Opacity = 0.4;

        try
        {
            await DragDrop.DoDragDropAsync(e, data, DragDropEffects.Move);
        }
        finally
        {
            ClearHighlight();
            column.Opacity = 1;
        }
    }

    private Border? highlightedBorder;
    private bool? highlightedInsertBefore;
    private IBrush? normalBorderBrush;

    private void ClearHighlight()
    {
        if (highlightedBorder is null)
            return;

        highlightedBorder.BorderBrush = normalBorderBrush;
        highlightedBorder.BoxShadow = default;
        highlightedBorder = null;
    }

    private void Highlight(Border border, bool? insertBefore)
    {
        if (highlightedBorder == border && highlightedInsertBefore == insertBefore)
            return;

        ClearHighlight();
        normalBorderBrush = border.BorderBrush;

        highlightedBorder = border;
        highlightedInsertBefore = insertBefore;

        if (insertBefore is null)
        {
            border.BorderBrush = Brushes.DodgerBlue;
            return;
        }

        // inset shadow marks the drop edge without touching layout, so nothing moves on hover
        border.BoxShadow = new BoxShadows(new BoxShadow
        {
            OffsetX = insertBefore.Value ? 4 : -4,
            IsInset = true,
            Color = Colors.DodgerBlue,
        });
    }

    private void UpdateDropTarget(object? sender, DragEventArgs e)
    {
        var border = e.Source is Control source ? FindColumnBorder(source) : null;

        if (e.DataTransfer.TryGetValue(ColumnFormat) is { } draggedColumn)
        {
            if (border is null || border.DataContext == draggedColumn)
            {
                ClearHighlight();
                e.DragEffects = DragDropEffects.None;
                return;
            }

            Highlight(border, e.GetPosition(border).X < border.Bounds.Width / 2);
            e.DragEffects = DragDropEffects.Move;
            return;
        }

        if (e.DataTransfer.TryGetValue(ItemFormat) is not null && border is not null)
        {
            Highlight(border, null);
            e.DragEffects = DragDropEffects.Move;
            return;
        }

        ClearHighlight();
        e.DragEffects = DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Source is not Control source)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        if (DataContext is not KanbanViewModel viewModel)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        ClearHighlight();

        if (e.DataTransfer.TryGetValue(ColumnFormat) is { } draggedColumn)
        {
            var border = FindColumnBorder(source);
            var targetColumn = border?.DataContext as ColumnViewModel;

            if (border is null || targetColumn is null || targetColumn == draggedColumn)
            {
                e.DragEffects = DragDropEffects.None;
                return;
            }

            bool insertBefore = e.GetPosition(border).X < border.Bounds.Width / 2;

            viewModel.MoveColumn(draggedColumn, targetColumn, insertBefore);
            e.DragEffects = DragDropEffects.Move;
            return;
        }

        var item = e.DataTransfer.TryGetValue(ItemFormat);

        if (item is null)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        var target = FindColumnBorder(source)?.DataContext as ColumnViewModel;

        if (target is null)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        foreach (var column in viewModel.Columns)
            if (column.Items.Remove(item))
                break;

        target.Items.Add(item);
        e.DragEffects = DragDropEffects.Move;
    }

    private static Border? FindColumnBorder(Control control)
    {
        Control? current = control;

        while (current is not null)
        {
            if (current is Border { DataContext: ColumnViewModel } border)
                return border;

            current = current.Parent as Control;
        }

        return null;
    }
}
