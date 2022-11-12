using System;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm
	{
		public WrapperVm(T source, bool checkSourceNull = false)
		{
			if (checkSourceNull && source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			Source = source;
		}

		protected T Source { get; }
	}
}
