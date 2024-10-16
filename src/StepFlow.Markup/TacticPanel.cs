using System.Drawing;
using StepFlow.Common.Exceptions;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;
using StepFlow.Markup.Services;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Markup;

internal sealed class TacticPanel : Panel
{
	private static Texture GetTexture(ItemKind item) => item switch
	{
		ItemKind.Fire => Texture.ItemFire,
		ItemKind.Poison => Texture.ItemPoison,
		ItemKind.Speed => Texture.ItemSpeed,
		_ => throw EnumNotSupportedException.Create(item),
	};

	public TacticPanel(IControl control, IDrawer drawer, PlayMasters playMasters)
		: base(control, drawer)
	{
		ArgumentNullException.ThrowIfNull(control);
		ArgumentNullException.ThrowIfNull(drawer);
		ArgumentNullException.ThrowIfNull(playMasters);

		PlayMasters = playMasters;
	}

	private PlayMasters PlayMasters { get; }

	private List<Button> Buttons { get; } = new List<Button>();

	public override void Update()
	{
		if (!Control.OnTactic())
		{
			return;
		}

		var playMaster = PlayMasters.Current;
		if (playMaster is null)
		{
			return;
		}

		var player = playMaster.Playground.Items.OfType<PlayerCharacter>().SingleOrDefault();
		if (player is null)
		{
			return;
		}

		var skills = player.Items;

		while (skills.Count < Buttons.Count)
		{
			Buttons.RemoveAt(Buttons.Count - 1);
		}

		while (skills.Count > Buttons.Count)
		{
			Buttons.Add(new(Control, Drawer));
		}

		const float SIZE = 50;
		for (var i = 0; i < skills.Count; i++)
		{
			var button = Buttons[i];
			button.Content = GetTexture(skills[i]);
			button.ContentMargin = 5;
			button.Bounds = new(Bounds.X, Bounds.Y + i * SIZE, SIZE, SIZE);
			button.IsCheck = player.ActiveTarget == i;
		}

		if (Control.FreeOffset() != Point.Empty && Control.FreeSelect() is { } freeSelect)
		{
			foreach (var button in Buttons)
			{
				button.IsSelect = button.Bounds.Contains((PointF)freeSelect);
			}
		}
		else if (Control.CourseSelect() is { } courseSelect && courseSelect != SelectCourse.None)
		{
			var selectIndex = Buttons.FindIndex(x => x.IsSelect);
			var nextSelectButtonIndex = selectIndex >= 0 ? (selectIndex + 1) % Buttons.Count : 0;
			for (var i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].IsSelect = nextSelectButtonIndex == i;
			}
		}

		if (Control.IsSelect())
		{
			var playerCharacterProxy = (IPlayerCharacterProxy)playMaster.CreateProxy(player);
			for (var i = 0; i < Buttons.Count; i++)
			{
				var button = Buttons[i];
				button.IsCheck = button.IsSelect;
				if (button.IsCheck)
				{
					playerCharacterProxy.ActiveTarget = i;
				}
			}
		}
	}

	public override void Draw()
	{
		if (!Control.OnTactic())
		{
			return;
		}

		foreach (var button in Buttons)
		{
			button.Draw();
		}
	}
}
