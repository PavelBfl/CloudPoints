using System;
using StepFlow.Common;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseNotifyer
	{
		public WrapperVm(T source, bool checkSourceNull = false)
		{
			if (checkSourceNull && source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			Source = source;
		}

		internal T Source { get; }
	}
}
