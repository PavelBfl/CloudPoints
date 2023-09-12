namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class RemoveHandler : IHandler
	{
		public void Handle(IComponentProxy component, string eventName, object? args)
		{
			var playground = component.Subject.Playground;

			playground.Subjects.Remove(component.Subject);
		}
	}
}
