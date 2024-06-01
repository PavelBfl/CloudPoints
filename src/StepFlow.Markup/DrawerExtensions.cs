using System.Drawing;
using StepFlow.Markup.Services;

namespace StepFlow.Markup;

public static class DrawerExtensions
{
	public static void Button(this IDrawer drawer, Texture texture, RectangleF bounds, bool isSelect = false)
	{
		const float GAP = 5;

		ArgumentNullException.ThrowIfNull(drawer);

		var content = RectangleF.FromLTRB(
			bounds.Left + GAP,
			bounds.Top + GAP,
			bounds.Right - GAP,
			bounds.Bottom - GAP
		);

		var color = isSelect ? Color.Green : Color.White;

		drawer.Draw(new RectangleF(bounds.Location, new SizeF(bounds.Width, GAP)), color);
		drawer.Draw(new RectangleF(bounds.Location, new SizeF(GAP, bounds.Height)), color);
		drawer.Draw(new RectangleF(bounds.Right - GAP, bounds.Y, GAP, bounds.Height), color);
		drawer.Draw(new RectangleF(bounds.X, bounds.Bottom - GAP, bounds.Width, GAP), color);

		drawer.Draw(texture, content);
	}
}