using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.TimeLine
{
	public sealed class CommandsCollection<TCommand> : IReadOnlyList<TCommand>, ICommand
		where TCommand : notnull, ICommand
	{
		public CommandsCollection(IEnumerable<TCommand> items)
		{
			Items = (items ?? throw new System.ArgumentNullException(nameof(items))).ToArray();
		}

		public TCommand this[int index] => Items[index];

		public int Count => Items.Count;

		private IReadOnlyList<TCommand> Items { get; }

		public void Execute()
		{
			foreach (var command in this)
			{
				command.Execute();
			}
		}

		public void Revert()
		{
			foreach (var command in this.Reverse())
			{
				command?.Revert();
			}
		}

		public IEnumerator<TCommand> GetEnumerator() => Items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
