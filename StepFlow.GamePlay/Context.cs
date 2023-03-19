using System;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay
{
	public class Context
	{
		public Context()
		{
		}

		public Context(int colsCount, int rowsCount)
		{
			if (colsCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(colsCount));
			}

			if (rowsCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(rowsCount));
			}

			for (var iCol = 0; iCol < colsCount; iCol++)
			{
				for (var iRow = 0; iRow < rowsCount; iRow++)
				{
					World.Place.Add(new Node(this, new Point(iCol, iRow)));
				}
			}
		}

		public World World { get; } = new World();

		public Axis<Command> AxisTime { get; } = new Axis<Command>();

		public IList<Command> StaticCommands { get; } = new List<Command>();
	}
}
