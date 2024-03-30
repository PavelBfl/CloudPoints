using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;

namespace StepFlow.Master.Commands
{
	internal sealed class TurnResetCommand<TTarget> : AccessorCommand<TTarget, Turn?>
		where TTarget : class
	{
		public TurnResetCommand(TTarget target, IValueAccessor<TTarget, Turn?> accessor) : base(target, accessor)
		{
		}

		public override void Execute() => Accessor.SetValue(Target, null);

		public override void Revert() => Accessor.SetValue(Target, new Turn(0));
	}
}
