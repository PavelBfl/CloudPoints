using System.Drawing;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateSpeedItem : Executor<CreateSpeedItem.Parameters>
	{
		public CreateSpeedItem(PlayMaster playMaster) : base(playMaster, nameof(CreateSpeedItem))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().CreateSpeedItem(
				new Rectangle(parameters.X, parameters.Y, parameters.Width, parameters.Height),
				parameters.Speed
			);
		}

		public struct Parameters
		{
			public int X { get; set; }
			public int Y { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public int Speed { get; set; }
		}
	}
}
