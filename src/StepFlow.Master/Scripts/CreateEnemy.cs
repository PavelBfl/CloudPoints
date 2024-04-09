using System.Drawing;
using System.Numerics;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateEnemy : Executor<CreateEnemy.Parameters>
	{
		public CreateEnemy(PlayMaster playMaster) : base(playMaster, nameof(CreateEnemy))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playgroundProxy = (IPlaygroundProxy)PlayMaster.CreateProxy(PlayMaster.Playground);
			playgroundProxy.CreateEnemy(
				parameters.Bounds,
				parameters.Vision,
				parameters.Strategy,
				parameters.ReleaseItem,
				parameters.BeginVector
			);
		}

		public struct Parameters
		{
			public Rectangle Bounds { get; set; }
			public Rectangle Vision { get; set; }
			public Strategy Strategy { get; set; }
			public ItemKind ReleaseItem { get; set; }
			public Vector2 BeginVector { get; set; }
		}
	}
}
