namespace StepFlow.Master.Proxies
{
	public interface IReadOnlyProxyBase<out TTarget>
	{
		PlayMaster Owner { get; }

		TTarget Target { get; }
	}
}
