using System;
using System.Drawing;
using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies;

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
				Body = new Collided()
				{
					Current = { parameters.Bounds ?? Array.Empty<Rectangle>() },
					IsRigid = true,
				},
				Strength = parameters.Strength is { } strength ? Scale.CreateByMax(strength) : null,
			};

			PlayMaster.GetPlaygroundItemsProxy().Add(barrier);
		}

		public struct Parameters
		{
			public Rectangle[]? Bounds { get; set; }

			public int? Strength { get; set; }

			public ObstructionKind Kind { get; set; }

			public ObstructionView View { get; set; }
		}
	}
}
