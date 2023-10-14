using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	internal sealed class Event : ICollection<Handler>
	{
		public Event(Playground owner) => Owner = owner ?? throw new ArgumentNullException(nameof(owner));

		public int Count => Observers.Count;

		public bool IsReadOnly => false;

		private Playground Owner { get; }

		private HashSet<Handler> Observers { get; } = new HashSet<Handler>();

		public void Add(Handler item) => Observers.Add(item);

		public void Clear() => Observers.Clear();

		public bool Contains(Handler item) => Observers.Contains(item);

		public void CopyTo(Handler[] array, int arrayIndex) => Observers.CopyTo(array, arrayIndex);

		public IEnumerator<Handler> GetEnumerator() => Observers.GetEnumerator();

		public bool Remove(Handler item) => Observers.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
