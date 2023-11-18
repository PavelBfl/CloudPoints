using StepFlow.Core.Border;

namespace StepFlow.Core.Components
{
	public interface ICollided
	{
		IBordered? Current { get; set; }

		IBordered? Next { get; set; }

		bool IsMove { get; set; }
	}

	public sealed class Collided : Subject, ICollided
	{
		public Collided(Context owner) : base(owner)
		{
		}

		public IBordered? Current { get; set; }

		public IBordered? Next { get; set; }

		public bool IsMove { get; set; }
	}
}
