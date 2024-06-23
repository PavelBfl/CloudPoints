using System.Drawing;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;

namespace StepFlow.Master.Scripts
{
	public sealed class PlayerCharacterCreate : Executor<PlayerCharacterCreate.Parameters>
	{
		public PlayerCharacterCreate(PlayMaster playMaster) : base(playMaster, nameof(PlayerCharacterCreate))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var body = new Collided()
			{
				Current = { parameters.Bounds },
				IsRigid = true,
			};
			body.PositionSync();

			var playerCharacter = new PlayerCharacter()
			{
				Name = "Player",
				Strength = Scale.CreateByMax(parameters.Strength),
				Cooldown = Scale.Create(parameters.Cooldown),
				Body = body,
				Schedulers =
				{
					new SchedulerRunner()
					{
						Scheduler = new SchedulerVector()
						{
							Collided = body,
							Vectors =
							{
								new CourseVector()
								{
									Name = Material.SHEDULER_CONTROL_NAME,
								},
							},
						}
					},
				},
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
