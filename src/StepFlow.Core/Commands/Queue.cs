using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	internal class Queue<T> : IQueue<T>
	{
		public Queue(T target)
		{
			Target = target;
		}

		public T Target { get; }

		private List<ITargetingCommand<T>> Commands { get; } = new List<ITargetingCommand<T>>();

		public int Count => Commands.Count;

		public ITargetingCommand<T> this[int index] => Commands[index];

		public IReadOnlyCollection<ITargetingCommand<T>>? Dequeue()
		{
			var result = new List<ITargetingCommand<T>>();

			var i = 0;
			while (i < Commands.Count)
			{
				if (Commands[i].CanExecute())
				{
					result.Add(Commands[i]);
					Commands.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			return result;
		}

		public ITargetingCommand<T>? Add(IBuilder<T> builder)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			if (builder.CanBuild(Target))
			{
				var command = builder.Build(Target);
				Commands.Add(command);
				return command;
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
