namespace StepFlow.Core
{
	public interface IChild
	{
		Playground Owner { get; }

		uint Id { get; }
	}
}
