using System.Drawing;
using System.Linq;
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
			var playgroundProxy = (IPlaygroundProxy)PlayMaster.CreateProxy(PlayMaster.Playground);

			playgroundProxy.CreateObstruction(
				parameters.Bounds ?? Enumerable.Empty<Rectangle>(),
				parameters.Strength,
				parameters.Kind
			);
		}

		public struct Parameters
		{
			public Rectangle[]? Bounds { get; set; }

			public int? Strength { get; set; }

			public ObstructionKind Kind { get; set; }
		}
	}
}
