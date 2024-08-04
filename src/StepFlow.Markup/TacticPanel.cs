using System.Drawing;
using StepFlow.Common.Exceptions;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;
using StepFlow.Markup.Services;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Markup;

internal sealed class TacticPanel
{
	public TacticPanel(IControl control, IDrawer drawer, RectangleF place, PlayMasters playMasters)
	{
		ArgumentNullException.ThrowIfNull(control);
		ArgumentNullException.ThrowIfNull(drawer);
		ArgumentNullException.ThrowIfNull(playMasters);

		Control = control;
		Drawer = drawer;
		Place = place;
		PlayMasters = playMasters;
	}

	private IControl Control { get; }

	public IDrawer Drawer { get; }

	public RectangleF Place { get; }

	private PlayMasters PlayMasters { get; }

	private List<SkillButton> Buttons { get; } = new List<SkillButton>();

	public void Update()
	{
		var playMaster = PlayMasters.Current;
		if (playMaster is null)
		{
			return;
		}

		var skills = Control.OnTactic() ? Enum.GetValues<CharacterSkill>() : Array.Empty<CharacterSkill>();

		while (skills.Length < Buttons.Count)
		{
			Buttons.RemoveAt(Buttons.Count - 1);
		}

		while (skills.Length > Buttons.Count)
		{
			Buttons.Add(new());
		}

		const float SIZE = 50;
		for (var i = 0; i < skills.Length; i++)
		{
			var button = Buttons[i];
			button.Skill = skills[i];
			button.Bounds = new(Place.X, Place.Y + i * SIZE, SIZE, SIZE);
		}

		// TODO Temp
		if (!playMaster.Playground.Items.OfType<PlayerCharacter>().Any())
		{
			return;
		}

		var playerCharacterGui = (IPlayerCharacterProxy)playMaster.CreateProxy(playMaster.Playground.GetPlayerCharacterRequired());
		if (Control.FreeSelect() is { } freeSelect)
		{
			foreach (var button in Buttons)
			{
				button.IsSelect = button.Bounds.Contains((PointF)freeSelect);
				if (button.IsSelect)
				{
					if (Control.SelectMain())
					{
						playerCharacterGui.MainSkill = button.Skill;
					}

					if (Control.SelectAuxiliary())
					{
						playerCharacterGui.AuxiliarySkill = button.Skill;
					}
				}
			}
		}
		else
		{
			// TODO Сделать ручное управление
		}

		foreach (var button in Buttons)
		{
			button.IsMain = button.Skill == playerCharacterGui.MainSkill;
			button.IsAuxiliary = button.Skill == playerCharacterGui.AuxiliarySkill;
		}
	}

	public void Draw()
	{
		foreach (var button in Buttons)
		{
			Drawer.Button(button.Icon, button.Bounds, button.IsSelect);
			if (button.IsMain)
			{
				Drawer.Draw(new RectangleF(button.Bounds.Location, new SizeF(10, 10)), Color.Red);
			}

			if (button.IsAuxiliary)
			{
				Drawer.Draw(new RectangleF(new PointF(button.Bounds.Right - 10, button.Bounds.Y), new SizeF(10, 10)), Color.Red);
			}
		}
	}

	private sealed class SkillButton
	{
		public CharacterSkill Skill { get; set; }

		public RectangleF Bounds { get; set; }

		public bool IsSelect { get; set; }

		public bool IsMain { get; set; }

		public bool IsAuxiliary { get; set; }

		public Texture Icon => Skill switch
		{
			CharacterSkill.Projectile => Texture.ItemFire,
			CharacterSkill.Arc => Texture.ItemPoison,
			CharacterSkill.Push => Texture.ItemSpeed,
			CharacterSkill.Dash => Texture.ItemUnknown,
			_ => throw EnumNotSupportedException.Create(Skill),
		};
	}
}