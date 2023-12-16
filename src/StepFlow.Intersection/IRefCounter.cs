namespace StepFlow.Intersection
{
	public interface IRefCounter<out T>
	{
		T Value { get; }

		void AddRef();

		void RemoveRef();
	}
}