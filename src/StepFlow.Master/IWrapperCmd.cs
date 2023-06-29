namespace StepFlow.Master
{
	public interface IWrapperCmd<out T>
	{
		T Source { get; }
	}
}
