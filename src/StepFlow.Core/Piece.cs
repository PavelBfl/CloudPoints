namespace StepFlow.Core
{
	public class Piece : Particle
	{
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
	}
}
