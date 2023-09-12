namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentProxy : IComponentController
	{
		ISubjectProxy Subject { get; }
	}
}
