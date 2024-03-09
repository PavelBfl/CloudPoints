namespace StepFlow.Master.Proxies
{
	public interface IProxyBase<out TTarget>
		where TTarget : class
	{
		PlayMaster Owner { get; }

		TTarget Target { get; }
	}
}
