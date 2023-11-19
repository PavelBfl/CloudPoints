using StepFlow.Core.Border;

namespace StepFlow.Core.Components
{
	public sealed class Collided : Subject
	{
		public IBordered? Current { get; set; }

		public IBordered? Next { get; set; }

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }
	}
}
