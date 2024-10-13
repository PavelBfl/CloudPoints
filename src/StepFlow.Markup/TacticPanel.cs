﻿using System.Drawing;
using System.Numerics;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;
using StepFlow.Markup.Services;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Markup;

internal sealed class TacticPanel : Panel
{
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

	private Vector2? PrevFreeSelect { get; set; }

	public override void Update()
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
			Buttons.Add(new(Control, Drawer));
		}

		const float SIZE = 50;
		for (var i = 0; i < skills.Length; i++)
		{
			var button = Buttons[i];
			button.Content = Texture.ItemFire;
			button.ContentMargin = 5;
			button.Bounds = new(Bounds.X, Bounds.Y + i * SIZE, SIZE, SIZE);
		}

		// TODO Temp
		if (!playMaster.Playground.Items.OfType<PlayerCharacter>().Any())
		{
			return;
		}

		var playerCharacterGui = (IPlayerCharacterProxy)playMaster.CreateProxy(playMaster.Playground.GetPlayerCharacterRequired());

		var currentFreeSelect = Control.FreeSelect();
		if (PrevFreeSelect != currentFreeSelect && currentFreeSelect is { } freeSelect)
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

		PrevFreeSelect = currentFreeSelect;

		if (Control.IsSelect())
		{
			var selectButton = Buttons.FindIndex(x => x.IsSelect);
			for (var i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].IsCheck = selectButton == i;
			}
		}
	}

	public override void Draw()
	{
		foreach (var button in Buttons)
		{
			button.Draw();
		}
	}
}
