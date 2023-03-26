using StepFlow.TimeLine.Exceptions;

namespace StepFlow.TimeLine
{
	public abstract class CommandBase : ICommand
	{
		public bool IsCompleted { get; private set; }

		public virtual void Execute()
		{
			if (IsCompleted)
			{
				throw ExceptionBuilder.CreateExecuteCompleteCommand();
			}

			ExecuteInner();
			IsCompleted = true;
		}

		protected abstract void ExecuteInner();
	}
}
