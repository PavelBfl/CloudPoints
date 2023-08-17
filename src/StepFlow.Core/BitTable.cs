using System;
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

		private BitArray[] Points { get; set; } = Array.Empty<BitArray>();

		private Size Size
		{
			get => new Size(Points.Length, Points.Length > 0 ? Points[0].Length : 0);
			set
			{
				var oldSize = Size;
				if (oldSize != value)
				{
					var sizeMin = new Size(Math.Min(oldSize.Width, value.Width), Math.Min(oldSize.Height, value.Height));
					var oldPoints = Points;
					Points = new BitArray[value.Width];
					for (var i = 0; i < Points.Length; i++)
					{
						Points[i] = new BitArray(value.Height);
					}

					for (var iX = 0; iX < sizeMin.Width; iX++)
					{
						for (var iY = 0; iY < sizeMin.Height; iY++)
						{
							Points[iX][iY] = oldPoints[iX][iY];
						}
					}
				}
			}
		}

		public Point Offset { get; set; }

		public void Add(Rectangle rectangle)
		{
			
		}
	}
}
