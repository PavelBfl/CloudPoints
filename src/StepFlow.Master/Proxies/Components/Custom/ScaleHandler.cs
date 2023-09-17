namespace StepFlow.Master.Proxies.Components.Custom
{
	internal class ScaleHandler : HandlerBase
	{
		public ScaleHandler(PlayMaster owner) : base(owner)
		{
		}

		protected override void HandleInner(IComponentProxy component)
		{
			if (((IScaleProxy)component).Value <= 0)
			{
				Subject.Playground.Subjects.Remove(Subject);
			}
		}
	}
}
