namespace StepFlow.TimeLine
{
	public class CommandBase : ICommand
	{
		public virtual void Dispose()
		{
		}

		public virtual void Execute()
		{
		}

		public virtual bool Prepare() => true;
	}
}
