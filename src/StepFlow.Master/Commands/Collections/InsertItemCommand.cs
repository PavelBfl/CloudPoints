using System.Collections.Generic;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands.Collections
{
	public class InsertItemCommand<TItem> : TargetingCommand<IList<TItem>>
	{
		public InsertItemCommand(IList<TItem> target, TItem newItem, int index) : base(target)
		{
			NewItem = newItem;
			Index = index;
		}

		private TItem NewItem { get; }

		private int Index { get; }

		public override void Execute() => Target.Insert(Index, NewItem);

		public override void Revert() => Target.RemoveAt(Index);
	}
}
