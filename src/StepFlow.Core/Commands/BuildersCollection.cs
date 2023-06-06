using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	internal class BuildersCollection<T> : IBuildersCollection<T>
	{
		public BuildersCollection(TargetingContainer<T> container) => Container = container ?? throw new ArgumentNullException(nameof(container));

		public ITargetingBuilder<T> this[IBuilder<T> key] => Builders[key];

		public IEnumerable<IBuilder<T>> Keys => Builders.Keys;

		public IEnumerable<ITargetingBuilder<T>> Values => Builders.Values;

		public int Count => Builders.Count;

		private TargetingContainer<T> Container { get; }

		private Dictionary<IBuilder<T>, ITargetingBuilder<T>> Builders { get; } = new Dictionary<IBuilder<T>, ITargetingBuilder<T>>();

		public bool Add(IBuilder<T> builder)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			return Builders.TryAdd(builder, new TargetingBuilder<T>(Container, builder));
		}

		public bool ContainsKey(IBuilder<T> key) => Builders.ContainsKey(key);

		public IEnumerator<KeyValuePair<IBuilder<T>, ITargetingBuilder<T>>> GetEnumerator() => Builders.GetEnumerator();

		public bool TryGetValue(IBuilder<T> key, out ITargetingBuilder<T> value) => Builders.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
