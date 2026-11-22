using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Kyanite.Dialogs.AppSettings;

public partial class AppSettingsView : UserControl
{
	public AppSettingsView()
	{
		InitializeComponent();
	}

	void SelectCategory(Control category)
	{
		var content = (SettingsScrollViewer.Content as Control)!;

		var position = category.TranslatePoint(new Point(0, 0), content)!;

		SettingsScrollViewer.Offset = new Vector(SettingsScrollViewer.Offset.X, position.Value.Y);
	}

	void GeneralClick(object? sender, RoutedEventArgs e)
	{
		SelectCategory(GeneralCategory);
	}

	void TodoModulesClick(object? sender, RoutedEventArgs e)
	{
		SelectCategory(TodoModulesCategory);
	}
}
