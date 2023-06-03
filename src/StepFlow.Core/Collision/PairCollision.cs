using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Collision
{
	public class PairCollision : IReadOnlyList<Piece>
	{
		public PairCollision(Piece first, Piece second)
		{
			First = first ?? throw new ArgumentNullException(nameof(first));
			Second = second ?? throw new ArgumentNullException(nameof(second));
		}

		public Piece this[int index] => index switch
		{
			0 => First,
			1 => Second,
			_ => throw new IndexOutOfRangeException(),
		};

		public int Count => 2;

		public Piece First { get; }
		public Piece Second { get; }

		public IEnumerator<Piece> GetEnumerator()
		{
			yield return First;
			yield return Second;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
