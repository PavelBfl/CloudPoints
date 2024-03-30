using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;

namespace StepFlow.Master.Commands
{
	internal sealed class TurnWaitCommand<TTarget> : AccessorCommand<TTarget, Turn?>
		where TTarget : class
	{
		public TurnWaitCommand(TTarget target, IValueAccessor<TTarget, Turn?> accessor) : base(target, accessor)
		{
		}

		public override void Execute() => Accessor.SetValue(Target, new Turn(1));

		public override void Revert() => Accessor.SetValue(Target, null);
	}
}
