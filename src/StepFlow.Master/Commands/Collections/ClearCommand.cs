using System.Collections.Generic;
using System.Linq;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands.Collections
{
	public class ClearCommand<TItem> : TargetingCommand<ICollection<TItem>>
	{
		public ClearCommand(ICollection<TItem> target) : base(target)
		{
			CacheItem = target.ToArray();
		}

		private TItem[] CacheItem { get; }

		public override void Execute() => Target.Clear();

		public override void Revert()
		{
			foreach (var item in CacheItem)
			{
				Target.Add(item);
			}
		}
	}
}
