using System.Drawing;
using StepFlow.Markup.Services;

namespace StepFlow.Markup;

internal class Panel
{
	public Panel(IControl control, IDrawer drawer)
	{
		ArgumentNullException.ThrowIfNull(control);
		ArgumentNullException.ThrowIfNull(drawer);

		Control = control;
		Drawer = drawer;
	}

	public IControl Control { get; }

	public IDrawer Drawer { get; }

	public RectangleF Bounds { get; set; }

	public virtual void Update()
	{
	}

	public virtual void Draw()
	{
	}
}
