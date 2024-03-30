using System;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands
{
	internal abstract class AccessorCommand<TTarget, TValue> : TargetingCommand<TTarget>
		where TTarget : class
	{
		protected AccessorCommand(TTarget target, IValueAccessor<TTarget, TValue> accessor) : base(target)
		{
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }
	}

	internal sealed class TurnIncrementCommand<TTarget> : TargetingCommand<TTarget>
		where TTarget : class
	{
		public TurnIncrementCommand(TTarget target, IValueAccessor<TTarget, Turn?> accessor) : base(target)
		{
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
		}

		public IValueAccessor<TTarget, Turn?> Accessor { get; }

		public override void Execute()
		{
			var turn = Accessor.GetValue(Target) ?? throw new InvalidOperationException();
			Accessor.SetValue(Target, new Turn(turn.Duration + 1, turn.Executor));
		}

		public override void Revert()
		{
			var turn = Accessor.GetValue(Target) ?? throw new InvalidOperationException();
			Accessor.SetValue(Target, new Turn(turn.Duration - 1, turn.Executor));
		}
	}
}
