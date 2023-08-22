using System.Collections.Generic;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands.Collections
{
	public class SetItemCommand<TItem> : TargetingCommand<IList<TItem>>
	{
		public SetItemCommand(IList<TItem> target, TItem newItem, int index) : base(target)
		{
			NewItem = newItem;
			OldItem = Target[index];
		}

		private TItem NewItem { get; }

		private TItem OldItem { get; }

		private int Index { get; }

		public override void Execute() => Target[Index] = NewItem;

		public override void Revert() => Target[Index] = OldItem;
	}
}
