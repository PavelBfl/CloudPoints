using System;

namespace StepFlow.TimeLine
{
	public class CommandWrapper<T> : CommandBase
		where T : ICommand
	{
		public CommandWrapper(T source) => Source = source ?? throw new ArgumentNullException(nameof(source));

		public T Source { get; }

		public override void Dispose() => Source.Dispose();

		public override void Execute() => Source.Execute();

		public override bool Prepare() => Source.Prepare();
	}

	public class CommandWrapper : CommandWrapper<ICommand>
	{
		public CommandWrapper(ICommand source) : base(source)
		{
		}
	}
}
