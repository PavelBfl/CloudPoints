using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core
{
	public abstract class Points : Subject, IEnumerable<Point>
	{
		public Points(Playground owner) : base(owner)
		{
		}

		public abstract Rectangle Bounds { get; }

		public abstract bool Contains(Point point);

		public Course Course { get; set; }

		public abstract void Offset(Course course);

		public abstract IEnumerator<Point> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
