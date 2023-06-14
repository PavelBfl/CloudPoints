using StepFlow.Core.Commands;

namespace StepFlow.Core
{
	public class Piece : Particle, IScheduled<Piece>
	{
		public Piece(Playground owner) : base(owner)
		{
			Scheduler = new Scheduler<Piece>(this);
		}

		public float CollisionDamage { get; set; }

		private Node? current;

		public Node? Current
		{
			get => current;
			set
			{
				CheckInteraction(value);

				if (Current != value)
				{
					Current?.Occupiers.Remove(this);

					current = value;

					Current?.Occupiers.Add(this);
				}
			}
		}

		private Node? next;

		public Node? Next
		{
			get => next;
			set
			{
				CheckInteraction(value);

				next = value;
			}
		}

		public bool IsScheduledStep { get; set; }

		public IScheduler<Piece> Scheduler { get; }
	}
}
