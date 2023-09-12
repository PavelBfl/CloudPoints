namespace StepFlow.Master.Proxies.Components.Custom
{
	public interface IHandler
	{
		void Handle(IComponentProxy component, string eventName, object? args);
	}
}
