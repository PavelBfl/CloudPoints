namespace StepFlow.TimeLine
{
	public interface ICommand
	{
		void Execute();

		void Revert();
	}
}
