using System.Drawing;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Domains.Components;

namespace StepFlow.Master.Scripts
{
	public sealed class PlayerCharacterCreate : Executor<PlayerCharacterCreate.Parameters>
	{
		public PlayerCharacterCreate(PlayMaster playMaster) : base(playMaster, nameof(PlayerCharacterCreate))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var body = new Collided(PlayMaster.Playground.Context)
			{
				Current = { parameters.Bounds },
				IsRigid = true,
			};

			var playerCharacter = new PlayerCharacter(PlayMaster.Playground.Context)
			{
				Name = "Player",
				Strength = Scale.CreateByMax(parameters.Strength),
				Cooldown = Scale.CreateByMin(parameters.Cooldown),
				Body = body,
				Speed = parameters.Speed,
				Course = new Vector2(0.05f, 0),
			};

			PlayMaster.GetPlaygroundItemsProxy().Add(playerCharacter);
		}

		public struct Parameters
		{
			public Rectangle Bounds { get; set; }

			public int Strength { get; set; }

			public int Speed { get; set; }

			public int Cooldown { get; set; }
		}
	}
}
