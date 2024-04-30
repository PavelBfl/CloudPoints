using System.Numerics;
using StepFlow.Core;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Scripts
{
	public sealed class PlayerCharacterSetCourse : Executor<PlayerCharacterSetCourse.Parameters>
	{
		public PlayerCharacterSetCourse(PlayMaster playMaster) : base(playMaster, nameof(PlayerCharacterSetCourse))
		{
		}

		public override void Execute(Parameters parameters)
		{
			var playerCharacterProxy = (IPlayerCharacterProxy)PlayMaster.CreateProxy(PlayMaster.Playground.GetPlayerCharacterRequired());
			playerCharacterProxy.SetCourse(parameters.Course);
		}

		public struct Parameters
		{
			public float? Course { get; set; }
		}
	}
}
