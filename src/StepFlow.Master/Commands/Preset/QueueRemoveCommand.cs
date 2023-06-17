using System;

namespace StepFlow.Core.Commands.Preset
{
	public class QueueRemoveCommand<T> : Command<T>
		where T : IScheduled<T>
	{
		public QueueRemoveCommand(T target, IResolver<T> resolver, ITargetingCommand<T> command) : base(target, resolver)
		{
			Command = command ?? throw new ArgumentNullException(nameof(command));
		}

		public ITargetingCommand<T> Command { get; }

		public override void Execute() => ((T)Target.Buffer).Scheduler.Queue.Remove(Command);

		public override void Revert() => ((T)Target.Buffer).Scheduler.Queue.Add(Command);
	}
}
