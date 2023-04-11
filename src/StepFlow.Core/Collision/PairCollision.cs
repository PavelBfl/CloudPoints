using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Collision
{
	public class PairCollision<TPiece> : IReadOnlyList<TPiece>
		where TPiece : Piece
	{
		public PairCollision(TPiece first, TPiece second)
		{
			First = first ?? throw new ArgumentNullException(nameof(first));
			Second = second ?? throw new ArgumentNullException(nameof(second));
		}

		public TPiece this[int index] => index switch
		{
			0 => First,
			1 => Second,
			_ => throw new IndexOutOfRangeException(),
		};

		public int Count => 2;

		public TPiece First { get; }
		public TPiece Second { get; }

		public IEnumerator<TPiece> GetEnumerator()
		{
			yield return First;
			yield return Second;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
