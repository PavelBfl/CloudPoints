using System.Drawing;
using StepFlow.Core.Components;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateDamageItem : Executor<CreateDamageItem.Parameters>
	{
		public CreateDamageItem(PlayMaster playMaster) : base(playMaster, nameof(CreateDamageItem))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().CreateDamageItem(
				new Rectangle(parameters.X, parameters.Y, parameters.Width, parameters.Height),
				parameters.Value,
				parameters.Kind
			);
		}

		public struct Parameters
		{
			public int X { get; set; }
			public int Y { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public int Value { get; set; }
			public DamageKind Kind { get; set; }
		}
	}
}
