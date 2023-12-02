using System;
using System.Collections.Generic;

namespace StepFlow.Master.Scripts
{
	public abstract class Executor
	{
		protected Executor(string name)
			=> Name = name ?? throw new ArgumentNullException(nameof(name));

		public string Name { get; }

		public abstract void Execute(IReadOnlyDictionary<string, object>? parameters);
	}
}
