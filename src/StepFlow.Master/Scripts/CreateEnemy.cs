using System.Drawing;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateEnemy : Executor<CreateEnemy.Parameters>
	{
		public CreateEnemy(PlayMaster playMaster) : base(playMaster, nameof(CreateEnemy))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().CreateEnemy(
				new Rectangle(parameters.X, parameters.Y, parameters.Width, parameters.Height),
				new Rectangle(parameters.VisionX, parameters.VisionY, parameters.VisionWidth, parameters.VisionHeight),
				parameters.ReleaseItem
			);
		}

		public struct Parameters
		{
			public int X { get; set; }
			public int Y { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public int VisionX { get; set; }
			public int VisionY { get; set; }
			public int VisionWidth { get; set; }
			public int VisionHeight { get; set; }
			public ItemKind ReleaseItem { get; set; }
		}
	}
}
