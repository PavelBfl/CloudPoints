using System;

namespace StepFlow.Master
{
	internal class WrapperCmd<TSource> : IWrapperCmd<TSource>
		where TSource : class
	{
		public WrapperCmd(PlayMaster owner, TSource source)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public PlayMaster Owner { get; }

		public TSource Source { get; }
	}
}
