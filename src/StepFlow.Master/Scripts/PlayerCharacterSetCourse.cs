using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Domains.Elements;
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
			const float SPEED = 0.05f;
			const float JUMP_FORCE = 0.05f;

			var playerCharacterProxy = (IPlayerCharacterProxy)PlayMaster.CreateProxy(PlayMaster.Playground.GetPlayerCharacterRequired());

			var x = parameters.Course switch
			{
				Horizontal.Left => -SPEED,
				Horizontal.Center => 0,
				Horizontal.Right => SPEED,
				_ => throw EnumNotSupportedException.Create(parameters.Course),
			};
			var y = parameters.Jump ? -JUMP_FORCE : playerCharacterProxy.Course.Y;
			playerCharacterProxy.Course = new Vector2(x, y);
		}

		public struct Parameters
		{
			public Horizontal Course { get; set; }

			public bool Jump { get; set; }
		}
	}
}
