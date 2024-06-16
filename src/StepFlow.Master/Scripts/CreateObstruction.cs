using System;
using System.Drawing;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateObstruction : Executor<CreateObstruction.Parameters>
	{
		public CreateObstruction(PlayMaster playMaster) : base(playMaster, nameof(CreateObstruction))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var barrier = new Obstruction()
			{
				Name = "Obstruction",
				Kind = parameters.Kind,
				View = parameters.View,
				Weight = parameters.Weight,
				Body = new Collided()
				{
					Current = { parameters.Bounds ?? Array.Empty<Rectangle>() },
					IsRigid = true,
				},
				Strength = parameters.Strength is { } strength ? Scale.CreateByMax(strength) : null,
			};
			barrier.Body.PositionSync();

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
