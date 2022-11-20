using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public interface ISubCommand : ICommand
	{
		Piece Owner { get; }
	}

	public class CommandsQueie
	{
		public Piece Owner { get; }

		public void Add(ICommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException();
			}


		}

		private void RemoveProtected(SubCommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}


		}

		private sealed class SubCommand : CommandBase, ISubCommand
		{
			public SubCommand(Piece owner, CommandsQueie ownerQueue)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				OwnerQueue = ownerQueue ?? throw new ArgumentNullException(nameof(ownerQueue));
			}

			public Piece Owner { get; }
			public CommandsQueie OwnerQueue { get; }

			public override void Dispose()
			{
				base.Dispose();

				OwnerQueue.RemoveProtected(this);
			}
		}
	}

	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{

		}

		private LocalQueue Queue { get; } = new LocalQueue();

		public IReadOnlyList<ICommand> CommandsQueue => Queue;

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
