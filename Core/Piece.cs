using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{

		}

		private LocalQueue Queue { get; } = new LocalQueue();

		public IReadOnlyList<ICommand> CommandsQueue => Queue;

		protected void Add(long time, ICommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			var localCommand = new LocalCommand(this, time, command);
			Queue.Add(localCommand.Time, localCommand);
			Owner.TimeAxis.Registry(time, localCommand);
		}

		private sealed class LocalQueue : SortedList<long, LocalCommand>, IReadOnlyList<ICommand>
		{
			ICommand IReadOnlyList<ICommand>.this[int index] => base[index];

			IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator() => Values.Select(x => x.Source).GetEnumerator();
		}

		private sealed class LocalCommand : ICommand
		{
			public LocalCommand(Piece owner, long time, ICommand source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Time = time;
				Source = source ?? throw new ArgumentNullException(nameof(source));
			}

			public Piece Owner { get; }
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
