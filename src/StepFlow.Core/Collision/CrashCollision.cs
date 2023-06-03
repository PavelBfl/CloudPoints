namespace StepFlow.Core.Collision
{
	public sealed class CrashCollision : PairCollision
	{
		public CrashCollision(Piece stationary, Piece moved)
			: base(stationary, moved)
		{
		}

		public Piece Stationary => First;
		public Piece Moved => Second;
	}
}
