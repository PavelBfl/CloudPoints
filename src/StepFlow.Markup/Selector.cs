using System.Drawing;
using StepFlow.Core.Elements;
using StepFlow.Markup.Services;
using StepFlow.Master;

namespace StepFlow.Markup;

public sealed class Selector
{
	public Selector(IDrawer drawer, IControl control, PlayMaster playMaster)
	{
		ArgumentNullException.ThrowIfNull(drawer);
		ArgumentNullException.ThrowIfNull(control);
		ArgumentNullException.ThrowIfNull(playMaster);

		Drawer = drawer;
		Control = control;
		PlayMaster = playMaster;
	}

	public IDrawer Drawer { get; }

	public IControl Control { get; }

	public PlayMaster PlayMaster { get; }

	public void Update()
	{
		
	}

	public void Draw()
	{
		const int SIZE = 50;
		var skills = Enum.GetValues<CharacterSkill>();

		for (var i = 0; i < skills.Length; i++)
		{
			Drawer.Draw(Texture.ProjectilePoison, new RectangleF(i * SIZE, 0, SIZE, SIZE));
		}
	}
}