using System;
using System.Collections.Generic;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands.Collections
{
	public class AddKeyCommand<TKey, TValue> : TargetingCommand<IDictionary<TKey, TValue>>
		where TKey : notnull
	{
		public AddKeyCommand(IDictionary<TKey, TValue> target, TKey key, TValue value) : base(target)
		{
			Key = key ?? throw new ArgumentNullException(nameof(key));
			Value = value;
		}

		private TKey Key { get; }

		private TValue Value { get; }

		public override void Execute() => Target.Add(Key, Value);

		public override void Revert() => Target.Remove(Key);
	}
}
