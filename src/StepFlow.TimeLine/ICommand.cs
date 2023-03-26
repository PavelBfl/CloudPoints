using System;

namespace StepFlow.TimeLine
{
	public interface ICommand
	{
		bool IsCompleted { get; }
		void Execute();
	}
}
