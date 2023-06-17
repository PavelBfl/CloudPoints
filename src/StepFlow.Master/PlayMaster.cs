using StepFlow.TimeLine;
using AdaptiveExpressions;
using System;
using StepFlow.Core;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public long Time { get; private set; }

		public Axis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public Playground Playground { get; } = new Playground();

		public void Execute(Expression expression)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}


		}
	}
}
