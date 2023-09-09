namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentProxy : IComponentController
	{
		IContainerProxy Container { get; }
	}
}
