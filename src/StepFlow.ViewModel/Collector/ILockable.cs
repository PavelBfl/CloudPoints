using System;
using System.Collections.Generic;

namespace StepFlow.ViewModel.Collector
{
	public interface ILockable : IDisposable
	{
		bool Lock { get; }

		IEnumerable<ILockable> GetContent();
	}
}
