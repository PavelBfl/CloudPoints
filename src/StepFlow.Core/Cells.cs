using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core
{
	public abstract class Cells : Subject, IEnumerable<ICell>
	{
		public Cells(Playground owner) : base(owner)
		{
		}

		public abstract Rectangle Bounds { get; }

		public abstract IEnumerator<ICell> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
