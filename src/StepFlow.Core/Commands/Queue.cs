using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace StepFlow.Core.Commands
{
	internal class Queue<T> : IQueue<T>
	{
		public Queue(T target)
		{
			Target = target;
		}

		public T Target { get; }

		public IReadOnlyCollection<ITargetingCommand<T>> this[long key] => Commands[key];

		public IEnumerable<long> Keys => Commands.Keys;

		public IEnumerable<IReadOnlyCollection<ITargetingCommand<T>>> Values => Commands.Values;

		public int Count => Commands.Count;

		private SortedDictionary<long, HashSet<ITargetingCommand<T>>> Commands { get; } = new SortedDictionary<long, HashSet<ITargetingCommand<T>>>();

		public bool ContainsKey(long key) => Commands.ContainsKey(key);

		public IReadOnlyCollection<ITargetingCommand<T>>? Dequeue(long key) => Commands.Remove(key, out var result) ? result : null;

		public ITargetingCommand<T>? Add(long key, IBuilder<T> builder)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			if (builder.CanBuild(Target))
			{
				if (!Commands.TryGetValue(key, out var localCommands))
				{
					localCommands = new HashSet<ITargetingCommand<T>>();
					Commands.Add(key, localCommands);
				}

				var command = builder.Build(Target);
				localCommands.Add(command);
				return command;
			}
			else
			{
				return null;
			}
		}

		public IEnumerator<KeyValuePair<long, IReadOnlyCollection<ITargetingCommand<T>>>> GetEnumerator()
			=> Commands.Select(x => new KeyValuePair<long, IReadOnlyCollection<ITargetingCommand<T>>>(x.Key, x.Value)).GetEnumerator();

		public bool TryGetValue(long key, [MaybeNullWhen(false)] out IReadOnlyCollection<ITargetingCommand<T>> value)
		{
			if (Commands.TryGetValue(key, out var result))
			{
				value = result;
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
