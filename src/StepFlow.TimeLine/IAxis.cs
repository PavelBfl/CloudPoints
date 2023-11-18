using System.Collections.Generic;

namespace StepFlow.TimeLine
{
	public interface IAxis<TCommand> : IReadOnlyList<TCommand>
		where TCommand : notnull, ICommand
	{
		void Add(TCommand command, bool isCompleted = false);
		bool Execute();
		bool Revert();
	}
}
