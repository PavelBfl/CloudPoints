using System.Collections.Generic;

namespace StepFlow.Master.Scripts
{
	public sealed class EmptyExecutor : Executor
	{
		public EmptyExecutor(string name, System.Action action) : base(name) => Action = action;

		public System.Action Action { get; }

		public override void Execute(IReadOnlyDictionary<string, object>? parameters) => Action();
	}
}
