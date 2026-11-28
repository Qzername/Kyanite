namespace Kyanite.Core.Dialogs;

public sealed record DialogSize
{
	public double? Width { get; init; }
	public double? Height { get; init; }
	public double? WidthRatio { get; init; }
	public double? HeightRatio { get; init; }
	public double MinWidth { get; init; } = 280;
	public double MinHeight { get; init; } = 160;
	public double MaxWidth { get; init; } = 720;
	public double MaxHeight { get; init; } = 600;

	public static DialogSize Fixed(double width, double height) => new()
	{
		Width = width,
		Height = height,
		MinWidth = width,
		MinHeight = height,
		MaxWidth = width,
		MaxHeight = height
	};

	public static DialogSize Responsive(
		double widthRatio = 0.85,
		double heightRatio = 0.8,
		double minWidth = 280,
		double minHeight = 160,
		double maxWidth = 720,
		double maxHeight = 600)
		=> new()
	{
		WidthRatio = Math.Clamp(widthRatio, 0.1, 1.0),
		HeightRatio = Math.Clamp(heightRatio, 0.1, 1.0),
		MinWidth = minWidth,
		MinHeight = minHeight,
		MaxWidth = maxWidth,
		MaxHeight = maxHeight
	};
}
