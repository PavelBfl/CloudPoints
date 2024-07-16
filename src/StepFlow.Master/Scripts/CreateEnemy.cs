using System.Drawing;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Domains.Components;
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

			var enemy = new Enemy()
			{
				Body = new Collided()
				{
					Current = { parameters.Bounds },
					IsRigid = true,
				},
				Vision = new Collided()
				{
					Current = { parameters.Vision },
				},
				Cooldown = Scale.CreateByMax(10000),
				Strength = Scale.CreateByMax(100),
				ReleaseItem = parameters.ReleaseItem,
				Course = parameters.Course,
				CollisionBehavior = parameters.CollisionBehavior,
				Speed = 10,
			};

			enemy.Body.PositionSync();
			enemy.Vision.PositionSync();

			if (parameters.States is { } states)
			{
				foreach (var state in states)
				{
					enemy.States.Add(state.ToState());
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
