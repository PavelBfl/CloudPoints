using System;
using System.Collections.Generic;

namespace TimeLine
{
	public class Axis
	{
		public long Current { get; private set; } = 0;

		private SortedDictionary<long, ICollection<ICommand>> Commands { get; } = new SortedDictionary<long, ICollection<ICommand>>();

		public void Registry(long time, ICommand command)
		{
			if (time <= Current)
			{
				throw new ArgumentOutOfRangeException(nameof(time));
			}

			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			if (Commands.TryGetValue(time, out var commands))
			{
				commands.Add(command);
			}
			else
			{
				Commands.Add(time, new HashSet<ICommand>()
				{
					command,
				});
			}
		}

		public bool MoveNext()
		{
			var nextStep = Current + 1;

			var commandsPrepare = true;
			if (Commands.Remove(nextStep, out var commands))
			{
				foreach (var command in commands)
				{
					commandsPrepare &= command.Prepare();
				}

				if (commandsPrepare)
				{
					foreach (var command in commands)
					{
						command.Execute();
					}

					foreach (var command in commands)
					{
						command.Dispose();
					}
				}
			}

			if (commandsPrepare)
			{
				Current = nextStep;
			}

			return commandsPrepare;
		}
	}
}
