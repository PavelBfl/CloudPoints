namespace StepFlow.Master.Proxies.Components.Custom
{
	public interface IHandler : IComponentProxy
	{
		void Handle(IComponentProxy component);
	}
}
