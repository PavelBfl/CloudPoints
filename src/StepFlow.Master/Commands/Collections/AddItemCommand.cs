using System.Collections.Generic;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands.Collections
{
	public class AddItemCommand<TItem> : TargetingCommand<ICollection<TItem>>
	{
		public AddItemCommand(ICollection<TItem> collection, TItem item) : base(collection)
		{
			Item = item;
		}

		private TItem Item { get; }

		public override void Execute() => Target.Add(Item);

		public override void Revert() => Target.Remove(Item);
	}
}
