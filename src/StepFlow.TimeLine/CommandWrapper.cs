using System;

namespace StepFlow.TimeLine
{
	public class CommandWrapper<T> : CommandBase
		where T : ICommand
	{
		public CommandWrapper(T source) => Source = source ?? throw new ArgumentNullException(nameof(source));

		public T Source { get; }

		protected override void ExecuteInner() => Source.Execute();
	}

	public class CommandWrapper : CommandWrapper<ICommand>
	{
		public CommandWrapper(ICommand source) : base(source)
		{
		}
	}
}
