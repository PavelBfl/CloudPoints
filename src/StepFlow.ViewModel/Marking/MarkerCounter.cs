using StepFlow.ViewModel.Exceptions;
using System.Collections.Generic;
using System;
using System.Collections;

namespace StepFlow.ViewModel.Marking
{
	public class MarkerCounter<T> : IReadOnlyDictionary<T, int>
	{
		public int this[T key] => Markers[key];

		public IEnumerable<T> Keys => Markers.Keys;

		public IEnumerable<int> Values => Markers.Values;

		public int Count => Markers.Count;

		private Dictionary<T, int> Markers { get; } = new Dictionary<T, int>();

		public bool ContainsKey(T key) => Markers.ContainsKey(key);

		public IEnumerator<KeyValuePair<T, int>> GetEnumerator() => Markers.GetEnumerator();

		public IDisposable Registry(T mark)
		{
			if (Markers.TryGetValue(mark, out var count))
			{
				Markers[mark] = count + 1;
			}
			else
			{
				Markers[mark] = 1;
				OnMarkChanged?.Invoke(this, new MarkChanged<T>(mark, MarkChangedState.Add));
			}

			return new Token(this, mark);
		}

		public bool TryGetValue(T key, out int value) => Markers.TryGetValue(key, out value);

		private void Unregistry(T mark)
		{
			var count = Markers[mark];

			if (count == 1)
			{
				Markers.Remove(mark);
				OnMarkChanged?.Invoke(this, new MarkChanged<T>(mark, MarkChangedState.Remove));
			}
			else if (count == 0)
			{
				throw InvalidViewModelException.CreateUnregistryMark();
			}
			else
			{
				Markers[mark] = --count;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public event EventHandler<MarkChanged<T>>? OnMarkChanged;

		private sealed class Token : IDisposable
		{
			public Token(MarkerCounter<T> owner, T key)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Key = key;
			}

			private MarkerCounter<T> Owner { get; }
			private T Key { get; }

			public void Dispose() => Owner.Unregistry(Key);
		}
	}
}
