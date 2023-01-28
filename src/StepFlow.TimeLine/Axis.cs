using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.TimeLine
{
	public class Axis<T>
		where T : notnull, ICommand
	{
		private const int OFFSET_NEXT = 1;
		private const int OFFSET_NEXT_PROCESSING = OFFSET_NEXT + 1;

		public long Current { get; private set; } = 0;

		public long NearestAllow => IsProcessing ? Current + OFFSET_NEXT_PROCESSING : Current + OFFSET_NEXT;

		public bool IsProcessing { get; private set; } = false;

		private SortedDictionary<long, HashSet<T>> TimeToCommand { get; } = new SortedDictionary<long, HashSet<T>>();

		private Dictionary<T, long> CommandToTime { get; } = new Dictionary<T, long>();

		public long GetTime(T command) => CommandToTime[command];

		public bool TryGetTime(T command, out long result) => CommandToTime.TryGetValue(command, out result);

		public void Registry(long time, T command)
		{
			if (time < NearestAllow)
			{
				throw new ArgumentOutOfRangeException(nameof(time));
			}

			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			if (!TimeToCommand.TryGetValue(time, out var commands))
			{
				commands = new HashSet<T>();
				TimeToCommand.Add(time, commands);
			}

			commands.Add(command);
			CommandToTime.Add(command, time);
		}

		public bool Remove(T command)
		{
			if (CommandToTime.Remove(command, out var time))
			{
				var localCommands = TimeToCommand[time];
				localCommands.Remove(command);
				if (!localCommands.Any())
				{
					TimeToCommand.Remove(time);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public bool MoveNext()
		{
			IsProcessing = true;

			var nextStep = Current + 1;

			var commandsPrepare = true;
			if (TimeToCommand.Remove(nextStep, out var commands))
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

			IsProcessing = false;

			return commandsPrepare;
		}
	}
}
