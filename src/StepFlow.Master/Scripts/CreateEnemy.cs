using System.Drawing;
using System.Numerics;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;
using StepFlow.Master.Proxies;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateEnemy : Executor<CreateEnemy.Parameters>
	{
		public CreateEnemy(PlayMaster playMaster) : base(playMaster, nameof(CreateEnemy))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playgroundProxy = (IPlaygroundProxy)PlayMaster.CreateProxy(PlayMaster.Playground);

			var enemy = new Enemy(PlayMaster.Playground.Context)
			{
				Body =
				{
					Current = PlayMaster.CreateShape(parameters.Bounds),
					IsRigid = true,
				},
				Vision = PlayMaster.CreateShape(parameters.Vision),
				Cooldown = Scale.CreateByMax(10000),
				Strength = Scale.CreateByMax(100),
				ReleaseItem = parameters.ReleaseItem,
				Course = parameters.Course,
				CollisionBehavior = parameters.CollisionBehavior,
				Speed = 10,
			};

			if (parameters.States is { } states)
			{
				foreach (var state in states)
				{
					enemy.States.Add(state.ToState(PlayMaster.Playground.Context));
				}
			}

			PlayMaster.GetPlaygroundItemsProxy().Add(enemy);
		}

		public struct Parameters
		{
			public Rectangle Bounds { get; set; }
			public Rectangle Vision { get; set; }
			public ItemKind ReleaseItem { get; set; }
			public Vector2 Course { get; set; }
			public CollisionBehavior CollisionBehavior { get; set; }
			public StateParameters[]? States { get; set; }
		}
	}
}
