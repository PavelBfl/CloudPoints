namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class RemoveSubjectHandler : HandlerBase
	{
		public RemoveSubjectHandler(PlayMaster owner) : base(owner)
		{
		}

		protected override void HandleInner(IComponentProxy component)
			=> Subject.Playground.Subjects.Remove(Subject);
	}
}
