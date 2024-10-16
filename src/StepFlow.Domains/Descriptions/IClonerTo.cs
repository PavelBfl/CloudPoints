namespace StepFlow.Domains.Descriptions
{
	public interface IClonerTo<in T>
	{
		void CloneTo(T container);
	}
}
