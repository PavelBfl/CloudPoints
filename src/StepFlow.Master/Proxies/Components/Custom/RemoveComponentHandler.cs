namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class RemoveComponentHandler : HandlerBase
	{
		public RemoveComponentHandler(PlayMaster owner) : base(owner)
		{
		}

		protected override void HandleInner(IComponentProxy component)
			=> component.Subject.RemoveComponent(component);
	}
}
