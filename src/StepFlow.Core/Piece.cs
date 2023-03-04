namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{
			StrengthMax = 10;
			Strength = 10;
			CollisionDamage = 5;
		}

		private Node? current;
		public Node? Current
		{
			get => current;
			set
			{
				if (Current != value)
				{
					Current?.Occupiers.Remove(this);

					current = value;

					Current?.Occupiers.Add(this);
				}
			}
		}

		public Node? Next { get; set; }

		internal override void TakeStep()
		{
			if (Next is { })
			{
				Current = Next;
				Next = null;
			}
		}

		public double CollisionDamage { get; set; }
	}
}
