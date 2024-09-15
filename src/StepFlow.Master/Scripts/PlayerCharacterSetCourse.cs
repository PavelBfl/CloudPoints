using System.Numerics;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
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
				HorizontalAlign.Left => -SPEED,
				HorizontalAlign.Center => 0,
				HorizontalAlign.Right => SPEED,
				_ => throw EnumNotSupportedException.Create(parameters.Course),
			};

			var y = parameters.Jump && playerCharacterProxy.CanJump() ? -JUMP_FORCE : playerCharacterProxy.Course.Y;
			playerCharacterProxy.Course = new Vector2(x, y);
		}

		public struct Parameters
		{
			public HorizontalAlign Course { get; set; }

			public bool Jump { get; set; }
		}
	}
}
