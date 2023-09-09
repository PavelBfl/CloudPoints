using System.Collections.Generic;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Collections
{
	public class DictionaryProxy<TKey, TValue, TDictionary, TValueProxy> : CollectionProxy<KeyValuePair<TKey, TValue>, TDictionary, KeyValuePair<TKey, TValueProxy>>, IDictionary<TKey, TValueProxy>
		where TValue : class
		where TValueProxy : ProxyBase<TValue>
		where TKey : notnull
		where TDictionary : class, IDictionary<TKey, TValue>
	{
		public DictionaryProxy(PlayMaster owner, TDictionary target) : base(owner, target)
		{
		}

		public TValueProxy this[TKey key]
		{
			get => (TValueProxy)Owner.CreateProxy(Target[key]);
			set
			{
				var command = ContainsKey(key) ?
					(ICommand)new SetKeyCommand<TKey, TValue>(Target, key, value.Target) :
					new AddKeyCommand<TKey, TValue>(Target, key, value.Target);

				Owner.TimeAxis.Add(command);
			}
		}

		public ICollection<TKey> Keys => Target.Keys;

		public ICollection<TValue> Values => Target.Values;

		public void Add(TKey key, TValueProxy value) => Owner.TimeAxis.Add(new AddKeyCommand<TKey, TValue>(Target, key, value.Target));

		public bool ContainsKey(TKey key) => Target.ContainsKey(key);

		public bool Remove(TKey key)
		{
			if (Target.TryGetValue(key, out var value))
			{
				Owner.TimeAxis.Add(new Reverse(new AddKeyCommand<TKey, TValue>(Target, key, value)));
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetValue(TKey key, out TValueProxy value)
		{
			if (Target.TryGetValue(key, out var itemTarget))
			{
				value = (TValueProxy)Owner.CreateProxy(itemTarget);
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}
	}
}
