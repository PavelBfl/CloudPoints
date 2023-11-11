namespace StepFlow.Master.Proxies
{
	public interface IProxyBase<out TTarget> : IReadOnlyProxyBase<TTarget>
		where TTarget : class
	{
		
	}
}
