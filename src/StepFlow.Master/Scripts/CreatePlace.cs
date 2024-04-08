using System.Drawing;
using StepFlow.Master.Proxies;

namespace StepFlow.Master.Scripts
{
	public sealed class CreatePlace : Executor<CreatePlace.Parameters>
	{
		public CreatePlace(PlayMaster playMaster) : base(playMaster, nameof(CreatePlace))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playgroundProxy = (IPlaygroundProxy)PlayMaster.CreateProxy(PlayMaster.Playground);
			playgroundProxy.CreatePlace(parameters.Bounds);
		}

		public struct Parameters
		{
			public Rectangle Bounds { get; set; }
		}
	}
}
