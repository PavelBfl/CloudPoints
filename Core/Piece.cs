using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TimeLine;

namespace Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{

		}

		private SortedList<int, ICommand> LocalQueue { get; } = new SortedList<int, ICommand>();

		private int IdCounter { get; set; } = 0;

		public IReadOnlyList<ICommand> CommandsQueue => new ReadOnlyCollection<ICommand>(LocalQueue.Values);

		protected void Add(ICommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			var localCommand = new LocalCommand(this, IdCounter++, command);
			LocalQueue.Add(localCommand.Id, localCommand);
		}

		private sealed class LocalCommand : ICommand
		{
			public LocalCommand(Piece owner, int id, ICommand source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Id = id;
				Source = source ?? throw new ArgumentNullException(nameof(source));
			}

			public Piece Owner { get; }
			public int Id { get; }
			public ICommand Source { get; }

			public void Dispose()
			{
				Owner.LocalQueue.Remove(Id);
				Source.Dispose();
			}

			public void Execute() => Source.Execute();

			public bool Prepare() => Source.Prepare();
		}
	}
}
