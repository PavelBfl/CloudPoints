namespace StepFlow.Master.Proxies.Components
{
	public interface IHandlerProxy : IComponentProxy
	{
		bool Disposable { get; set; }

		string? Reference { get; set; }

		void Handle(IComponentProxy component);
	}
}
