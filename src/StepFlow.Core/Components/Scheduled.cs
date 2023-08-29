using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public sealed class Scheduled : Component
	{
		public long Time { get; set; }

		public Course? Course { get; set; }
	}
}
