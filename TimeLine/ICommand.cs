using System;

namespace TimeLine
{
	public interface ICommand : IDisposable
	{
		bool Prepare();

		void Execute();
	}
}
