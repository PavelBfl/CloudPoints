using System;

namespace StepFlow.TimeLine
{
	public class CommandBase : ICommand
	{
		public virtual void Dispose()
		{
		}

		public virtual void Execute()
		{
		}

		public virtual bool Prepare() => true;
	}

	public class CommandWrapper : CommandBase
	{
		public CommandWrapper(ICommand source) => Source = source ?? throw new ArgumentNullException(nameof(source));

		public ICommand Source { get; }

		public override void Dispose() => Source.Dispose();

		public override void Execute() => Source.Execute();

		public override bool Prepare() => Source.Prepare();
	}
}
