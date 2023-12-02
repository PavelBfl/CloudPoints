using StepFlow.Core;

namespace StepFlow.Master.Scripts
{
	public sealed class PlayerCharacterSetCourse : Executor<PlayerCharacterSetCourse.Parameters>
	{
		public PlayerCharacterSetCourse(PlayMaster playMaster) : base(playMaster, nameof(PlayerCharacterSetCourse))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().PlayerCharacter?.SetCourse(parameters.Course);
		}

		public struct Parameters
		{
			public Course Course { get; set; }
		}
	}
}
