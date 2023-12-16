namespace StepFlow.Intersection
{
	public interface IRefContainer<in T>
	{
		void Add(T item);

		void Remove(T item);
	}
}