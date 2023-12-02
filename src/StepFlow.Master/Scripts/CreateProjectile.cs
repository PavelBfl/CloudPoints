using StepFlow.Core;

namespace StepFlow.Master.Scripts
{
	public sealed class CreateProjectile : Executor<CreateProjectile.Parameters>
	{
		public CreateProjectile(PlayMaster playMaster) : base(playMaster, nameof(CreateProjectile))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().PlayerCharacter?.CreateProjectile(parameters.Course);
		}

		public struct Parameters
		{
			public Course Course { get; set; }
		}
	}
}
