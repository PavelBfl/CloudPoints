using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Particle
	{
		public Particle(World owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public World Owner { get; }

		private LocalQueue Queue { get; } = new LocalQueue();

		public IReadOnlyList<ICommand> CommandsQueue => Queue;
		public IReadOnlyList<ICommand> SubCommandsQueue => new ReadOnlyCollection<LocalCommand>(Queue.Values);

		public void Add(ICommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			var localCommand = new LocalCommand(this, Owner.TimeAxis.Current + CommandsQueue.Count + 1, command);
			Queue.Add(localCommand.Time, localCommand);
			Owner.TimeAxis.Registry(localCommand.Time, localCommand);
		}

		private sealed class LocalQueue : SortedList<long, LocalCommand>, IReadOnlyList<ICommand>
		{
			ICommand IReadOnlyList<ICommand>.this[int index] => Values[index].Source;

			IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator() => Values.Select(x => x.Source).GetEnumerator();
		}

		private sealed class LocalCommand : ICommand
		{
			public LocalCommand(Particle owner, long time, ICommand source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Time = time;
				Source = source ?? throw new ArgumentNullException(nameof(source));
			}

			public Particle Owner { get; }
			public long Time { get; }
			public ICommand Source { get; }

			public void Dispose()
			{
				Owner.Queue.Remove(Time);
				Source.Dispose();
			}

			public void Execute() => Source.Execute();

			public bool Prepare() => Source.Prepare();
		}
	}
}
