using System.Drawing;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateItem : Executor<CreateItem.Parameters>
	{
		public CreateItem(PlayMaster playMaster) : base(playMaster, nameof(CreateItem))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playgroundProxy = (IPlaygroundProxy)PlayMaster.CreateProxy(PlayMaster.Playground);
			playgroundProxy.CreateItem(
				parameters.Position,
				parameters.Kind
			);
		}

		public struct Parameters
		{
			public Point Position { get; set; }

			public ItemKind Kind { get; set; }
		}
	}
}
