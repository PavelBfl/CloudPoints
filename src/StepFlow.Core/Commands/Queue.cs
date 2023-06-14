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

		private HashSet<ITargetingCommand<T>> Commands { get; } = new HashSet<ITargetingCommand<T>>();

		public int Count => Commands.Count;

		public bool IsReadOnly => throw new NotImplementedException();

		public IEnumerator<ITargetingCommand<T>> GetEnumerator() => Commands.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private void ItemValidate(ITargetingCommand<T> item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (!EqualityComparer<T>.Default.Equals(item.Target, Target))
			{
				throw new InvalidOperationException();
			}
		}

		public void Add(ITargetingCommand<T> item)
		{
			ItemValidate(item);

			Commands.Add(item);
		}

		public void Clear() => Commands.Clear();

		public bool Contains(ITargetingCommand<T> item)
		{
			ItemValidate(item);

			return Commands.Contains(item);
		}

		public void CopyTo(ITargetingCommand<T>[] array, int arrayIndex) => Commands.CopyTo(array, arrayIndex);

		public bool Remove(ITargetingCommand<T> item)
		{
			ItemValidate(item);

			return Commands.Remove(item);
		}
	}
}
