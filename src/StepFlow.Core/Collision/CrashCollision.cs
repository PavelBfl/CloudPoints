namespace StepFlow.Core.Collision
{
	public sealed class CrashCollision<TPiece> : PairCollision<TPiece>
		where TPiece : Piece
	{
		public CrashCollision(TPiece stationary, TPiece moved)
			: base(stationary, moved)
		{
		}

		public TPiece Stationary => First;
		public TPiece Moved => Second;
	}
}
