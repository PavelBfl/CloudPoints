using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.TimeLine
{
	public class Axis<TCommand> : IAxis<TCommand>
		where TCommand : notnull, ICommand
	{
		public int Current { get; private set; } = -1;

		private List<TCommand> Commands { get; } = new List<TCommand>();

		public bool? IsCompleted(TCommand command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			var index = Commands.IndexOf(command);
			if (index >= 0)
			{
				return index <= Current;
			}
			else
			{
				return null;
			}
		}

		public void Add(TCommand command, bool isCompleted = false)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			Trim();

			Commands.Add(command);

			if (isCompleted)
			{
				Current++;
			}
			else
			{
				Execute();
			}
		}

		public bool Execute()
		{
			var firstPlanedIndex = Current + 1;
			var canExecute = firstPlanedIndex < Commands.Count;

			if (canExecute)
			{
				Commands[firstPlanedIndex].Execute();
				Current++;
			}

			return canExecute;
		}

		public bool Revert()
		{
			var reverting = Current >= 0;

			if (reverting)
			{
				Commands[Current].Revert();
				Current--;
			}

			return reverting;
		}

		public bool Trim()
		{
			var firstPlanedIndex = Current + 1;
			var canTrim = firstPlanedIndex < Commands.Count;

			if (canTrim)
			{
				Commands.RemoveRange(firstPlanedIndex, Commands.Count - firstPlanedIndex);
			}

			return canTrim;
		}

		public int Count => Commands.Count;

		public TCommand this[int index] => Commands[index];

		public IEnumerator<TCommand> GetEnumerator() => Commands.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
