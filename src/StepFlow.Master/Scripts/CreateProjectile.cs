using StepFlow.Core;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateProjectile : Executor<CreateProjectile.Parameters>
	{
		public CreateProjectile(PlayMaster playMaster) : base(playMaster, nameof(CreateProjectile))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playerCharacterProxy = (IPlayerCharacterProxy)PlayMaster.CreateProxy(PlayMaster.Playground.GetPlayerCharacterRequired());
			playerCharacterProxy.CreateProjectile(parameters.Course);
		}

		public struct Parameters
		{
			public Course Course { get; set; }
		}
	}
}
