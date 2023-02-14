namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{
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
			Current = Next;
			Next = null;
		}
	}
}
