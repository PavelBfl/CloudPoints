using System.Collections.Generic;
using System.ComponentModel;
using StepFlow.Core.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Core.Components
{
	public sealed class Scheduled : Component
	{
		public IList<ICommand> Queue { get; } = new List<ICommand>();

		public IList<IBuilder> Builders { get; } = new List<IBuilder>();
	}
}
