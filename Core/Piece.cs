namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{
		}

		private HexNode? current;
		public HexNode? Current
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
	}
}
