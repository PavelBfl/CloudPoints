using System.Drawing;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateItem : Executor<CreateItem.Parameters>
	{
		public CreateItem(PlayMaster playMaster) : base(playMaster, nameof(CreateItem))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().CreateItem(
				new Point(parameters.X, parameters.Y),
				parameters.Kind
			);
		}

		public struct Parameters
		{
			public int X { get; set; }

			public int Y { get; set; }

			public ItemKind Kind { get; set; }
		}
	}
}
