using System.Drawing;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateObstruction : Executor<CreateObstruction.Parameters>
	{
		public CreateObstruction(PlayMaster playMaster) : base(playMaster, nameof(CreateObstruction))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var barrier = new Obstruction(PlayMaster.Playground.Context)
			{
				Name = "Obstruction",
				Kind = parameters.Kind,
				View = parameters.View,
				Weight = parameters.Weight,
				Body = new Collided(PlayMaster.Playground.Context)
				{
					Current = PlayMaster.CreateShape(parameters.Bounds),
					IsRigid = true,
				},
				Strength = parameters.Strength is { } strength ? Scale.CreateByMax(strength) : new Scale(),
			};

			PlayMaster.GetPlaygroundItemsProxy().Add(barrier);
		}

		public struct Parameters
		{
			public Rectangle[]? Bounds { get; set; }

			public int? Strength { get; set; }

			public int Weight { get; set; }

			public ObstructionKind Kind { get; set; }

			public ObstructionView View { get; set; }
		}
	}
}
