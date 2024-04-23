﻿using System.Drawing;
using StepFlow.Core;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class PlayerCharacterCreateProjectile : Executor<PlayerCharacterCreateProjectile.Parameters>
	{
		public PlayerCharacterCreateProjectile(PlayMaster playMaster) : base(playMaster, nameof(PlayerCharacterCreateProjectile))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playerCharacterProxy = (IPlayerCharacterProxy)PlayMaster.CreateProxy(PlayMaster.Playground.GetPlayerCharacterRequired());
			playerCharacterProxy.CreateProjectile(parameters.Course);
		}

		public struct Parameters
		{
			public Course Course { get; set; }

			public Point Push { get; set; }

			public int DefaultDamage { get; set; }

			public int Duration { get; set; }
		}
	}
}
