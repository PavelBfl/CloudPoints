using System;

namespace StepFlow.Intersection
{
	public class RefCounter<T> : IRefCounter<T>
	{
		public RefCounter(IRefContainer<T> container, T item)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
			Value = item;
		}

		private IRefContainer<T> Container { get; }

		public T Value { get; }

		private int Count { get; set; } = 0;

		public void AddRef()
		{
			Count++;

			if (Count == 1)
			{
				Container.Add(Value);
			}
		}

		public void RemoveRef()
		{
			Count--;

			if (Count < 0)
			{
				throw new InvalidOperationException();
			}
			else if (Count == 0)
			{
				Container.Remove(Value);
			}
		}
	}
}