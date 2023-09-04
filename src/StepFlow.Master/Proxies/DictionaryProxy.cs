using System.Collections.Generic;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	public class DictionaryProxy<TKey, TValue, TDictionary> : CollectionProxy<KeyValuePair<TKey, TValue>, TDictionary>, IDictionary<TKey, TValue>
		where TKey : notnull
		where TDictionary : class, IDictionary<TKey, TValue>
	{
		public DictionaryProxy(PlayMaster owner, TDictionary target) : base(owner, target)
		{
		}

		public TValue this[TKey key]
		{
			get => Target[key];
			set
			{
				var command = ContainsKey(key) ?
					(ICommand)new SetKeyCommand<TKey, TValue>(Target, key, value) :
					new AddKeyCommand<TKey, TValue>(Target, key, value);

				Owner.TimeAxis.Add(command);
			}
		}

		public ICollection<TKey> Keys => Target.Keys;

		public ICollection<TValue> Values => Target.Values;

		public void Add(TKey key, TValue value) => Owner.TimeAxis.Add(new AddKeyCommand<TKey, TValue>(Target, key, value));

		public bool ContainsKey(TKey key) => Target.ContainsKey(key);

		public bool Remove(TKey key)
		{
			if (TryGetValue(key, out var value))
			{
				Owner.TimeAxis.Add(new Reverse(new AddKeyCommand<TKey, TValue>(Target, key, value)));
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetValue(TKey key, out TValue value) => Target.TryGetValue(key, out value);
	}
}
