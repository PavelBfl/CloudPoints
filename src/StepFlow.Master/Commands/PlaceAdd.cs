using System;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands
{
	internal sealed class PlaceAdd : ICommand
	{
		public PlaceAdd(Place target, Node newItem)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			NewItem = newItem ?? throw new ArgumentNullException(nameof(newItem));
		}

		private Place Target { get; }

		private Node NewItem { get; }

		public void Execute() => Target.Add(NewItem);

		public void Revert() => Target.Remove(NewItem.Position);
	}
}
