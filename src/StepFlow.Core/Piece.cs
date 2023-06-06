using StepFlow.Core.Commands;
using System.Collections.Generic;

namespace StepFlow.Core
{
	public class Piece : Particle
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
			set => SetNext(value, true);
		}

		private void SetNext(Node? value, bool scheduled)
		{
			CheckInteraction(value);

			next = value;

			if (scheduled)
			{
				IsScheduledStep = true;
			}
		}

		public bool IsScheduledStep { get; set; }

		public void TakeStep()
		{
			if (IsScheduledStep)
			{
				Current = Next;
				Clear();
			}
		}

		public void Clear()
		{
			SetNext(null, false);
			IsScheduledStep = false;
		}

		public IScheduler<Piece> Scheduler { get; }
	}
}
