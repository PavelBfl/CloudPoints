﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{

		}

		private SortedList<long, ICommand> LocalQueue { get; } = new SortedList<long, ICommand>();

		public IReadOnlyList<ICommand> CommandsQueue => new ReadOnlyCollection<ICommand>(LocalQueue.Values);

		protected void Add(long time, ICommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			var localCommand = new LocalCommand(this, time, command);
			LocalQueue.Add(localCommand.Time, localCommand);
			Owner.TimeAxis.Registry(time, localCommand);
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
				Owner.LocalQueue.Remove(Time);
				Source.Dispose();
			}

			public void Execute() => Source.Execute();

			public bool Prepare() => Source.Prepare();
		}
	}
}
