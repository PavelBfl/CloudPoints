using System;

namespace StepFlow.TimeLine
{
	public sealed class Reverse : ICommand
	{
		public Reverse(ICommand target) => Target = target ?? throw new ArgumentNullException(nameof(target));

		private ICommand Target { get; }

		public void Execute() => Target.Revert();

		public void Revert() => Target.Execute();
	}
}
