using System;
using System.Collections.Generic;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands
{
	internal sealed class CollectionAdd<T> : ICommand
	{
		public CollectionAdd(ICollection<T> target, T newItem)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			NewItem = newItem;
		}

		private ICollection<T> Target { get; }

		private T NewItem { get; }

		public void Execute() => Target.Add(NewItem);

		public void Revert() => Target.Remove(NewItem);
	}
}
