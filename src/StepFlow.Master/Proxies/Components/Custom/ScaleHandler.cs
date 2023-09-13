namespace StepFlow.Master.Proxies.Components.Custom
{
	internal class ScaleHandler : ComponentMaster, IScaleHandler
	{
		public ScaleHandler(PlayMaster owner) : base(owner)
		{
		}

		public virtual void ValueChange(IScaleProxy component, string name)
		{
			if (component.Value <= 0)
			{
				Subject.Playground.Subjects.Remove(Subject);
			}
		}
	}
}
