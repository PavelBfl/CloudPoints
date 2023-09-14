using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	internal sealed class Event : ICollection<IComponentChild>
	{
		public Event(Playground owner) => Owner = owner ?? throw new ArgumentNullException(nameof(owner));

		public int Count => Observers.Count;

		public bool IsReadOnly => false;

		private Playground Owner { get; }

		private HashSet<IComponentChild> Observers { get; } = new HashSet<IComponentChild>();

		public void Add(IComponentChild item) => Observers.Add(item);

		public void Clear() => Observers.Clear();

		public bool Contains(IComponentChild item) => Observers.Contains(item);

		public void CopyTo(IComponentChild[] array, int arrayIndex) => Observers.CopyTo(array, arrayIndex);

		public IEnumerator<IComponentChild> GetEnumerator() => Observers.GetEnumerator();

		public bool Remove(IComponentChild item) => Observers.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
