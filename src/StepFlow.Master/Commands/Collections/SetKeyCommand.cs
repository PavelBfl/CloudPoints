using System;
using System.Collections.Generic;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands.Collections
{
	public class SetKeyCommand<TKey, TValue> : TargetingCommand<IDictionary<TKey, TValue>>
		where TKey : notnull
	{
		public SetKeyCommand(IDictionary<TKey, TValue> target, TKey key, TValue newValue) : base(target)
		{
			Key = key ?? throw new ArgumentNullException(nameof(key));
			NewValue = newValue;
			OldValue = Target[Key];
		}

		private TKey Key { get; }

		private TValue NewValue { get; }

		private TValue OldValue { get; }

		public override void Execute() => Target[Key] = NewValue;

		public override void Revert() => Target[Key] = OldValue;
	}
}
