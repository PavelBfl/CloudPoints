namespace StepFlow.Domains.Descriptions
{
	public interface IClonerProperty<out T>
	{
		void Clear();

		bool IsEmpty();

		T GetOrCreate();

		T Value { get; }
	}
}
