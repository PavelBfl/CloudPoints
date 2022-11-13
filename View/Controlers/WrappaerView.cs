using System;

namespace StepFlow.View.Controlers
{
	public class WrappaerView<T> : ViewBase
	{
		public WrappaerView(Game1 game, T source, bool checkSourceNull = false)
			: base(game)
		{
			if (checkSourceNull && source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			Source = source;
		}

		public T Source { get; }
	}
}
