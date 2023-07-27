using System.Collections;
using System.Drawing;

namespace StepFlow.Core
{
	public sealed class BitTable
	{
		public BitTable(Size size)
		{
			Points = new BitArray[size.Height];
			for (var i = 0; i < Points.Length; i++)
			{
				Points[i] = new BitArray(size.Width);
			}
		}

		private BitArray[] Points { get; }

		public Point Offset { get; set; }

		public bool this[Point point]
		{
			get => Points[point.Y][point.X];
			set => Points[point.Y][point.X] = value;
		}
	}
}
