namespace StepFlow.Core.Commands
{
	public interface IResolverBuilder<T>
	{
		IResolver<T> Build(T target);
	}
}
