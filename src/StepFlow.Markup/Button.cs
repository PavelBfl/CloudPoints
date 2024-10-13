using System.Drawing;
using StepFlow.Common.Drawing.Layout;
using StepFlow.Markup.Services;

namespace StepFlow.Markup;

internal sealed class Button : Panel
{
	public Button(IControl control, IDrawer drawer)
		: base(control, drawer)
	{
	}

	public Texture? Content { get; set; }

	public float ContentMargin { get; set; }

	public bool IsCheck { get; set; }

	public bool IsSelect { get; set; }

	public override void Draw()
	{
		var boundsColor = GetBoundsColor();

		Drawer.Draw(new RectangleF(Bounds.Location, new SizeF(ContentMargin, Bounds.Height)), boundsColor);
		Drawer.Draw(new RectangleF(Bounds.Location, new SizeF(Bounds.Width, ContentMargin)), boundsColor);
		Drawer.Draw(RectangleF.FromLTRB(Bounds.Right - ContentMargin, Bounds.Top, Bounds.Right, Bounds.Bottom), boundsColor);
		Drawer.Draw(RectangleF.FromLTRB(Bounds.Left, Bounds.Bottom - ContentMargin, Bounds.Right, Bounds.Bottom), boundsColor);

		if (Content is { } content)
		{
			Drawer.Draw(content, Bounds.Offset(ContentMargin));
		}
	}

	private Color GetBoundsColor()
	{
		if (IsCheck)
		{
			return IsSelect ? Color.Red : Color.DarkRed;
		}
		else
		{
			return IsSelect ? Color.Gray : Color.DarkGray;
		}
	}
}