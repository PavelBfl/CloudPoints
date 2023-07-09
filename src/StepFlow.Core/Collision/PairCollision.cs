using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerator<Piece> GetEnumerator() => new[] { First, Second }.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
