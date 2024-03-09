using System.Drawing;

namespace StepFlow.Master.Scripts
{
	public sealed class CreatePlace : Executor<CreatePlace.Parameters>
	{
		public CreatePlace(PlayMaster playMaster) : base(playMaster, nameof(CreatePlace))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().CreatePlace(parameters.Bounds);
		}

		public struct Parameters
		{
			public Rectangle Bounds { get; set; }
		}
	}
}
