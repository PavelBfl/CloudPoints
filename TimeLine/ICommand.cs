using System;

namespace StepFlow.TimeLine
{
	public interface ICommand : IDisposable
	{
		bool Prepare();

		void Execute();
	}
}
