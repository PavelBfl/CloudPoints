using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	internal class Queue<T> : IQueue<T>
	{
		public Queue(IList<ITargetingCommand<T>> commands) => Commands = commands ?? throw new ArgumentNullException(nameof(commands));

		private IList<ITargetingCommand<T>> Commands { get; }

		public ITargetingCommand<T> this[int index] => Commands[index];

		public int Count => Commands.Count;

		public ITargetingCommand<T>? Dequeue()
		{
			if (Commands.Count > 0)
			{
				var current = Commands[0];
				Commands.RemoveAt(0);
				return current;
			}
			else
			{
				return null;
			}
		}

		public IEnumerator<ITargetingCommand<T>> GetEnumerator() => Commands.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
