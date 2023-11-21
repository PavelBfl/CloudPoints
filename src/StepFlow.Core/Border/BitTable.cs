using System;
using System.Collections;
using System.Drawing;

namespace StepFlow.Core.Border
{
	public sealed class BitTable
	{
		public BitTable(Size size)
		{
			Points = new BitArray[size.Width];
			for (var i = 0; i < Points.Length; i++)
			{
				Points[i] = new BitArray(size.Height);
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

		private bool GetSafeFlag(Point point)
		{
			if (0 <= point.X && point.X < Size.Width && 0 <= point.Y && point.Y < Size.Height)
			{
				return Points[point.X][point.Y];
			}
			else
			{
				return false;
			}
		}

		public Bordered CreateBordered()
		{
			var result = new Bordered();

			for (var iX = 0; iX < Size.Width; iX++)
			{
				for (var iY = 0; iY < Size.Height; iY++)
				{
					var prevLeft = new Point(iX - 1, iY);
					var prevTop = new Point(iX, iY - 1);

					if (
						(!GetSafeFlag(prevLeft) || result.Contains(prevLeft)) && 
						(!GetSafeFlag(prevTop) || result.Contains(prevTop))
					)
					{
						var cell = new Cell()
						{
							Border = ExpandFromLeft(new Point(iX, iY)),
						};
						result.Childs.Add(cell);
					}
				}
			}

			return result;
		}

		private Rectangle ExpandFromLeft(Point begin)
		{
			var top = begin.Y;
			for (var i = begin.Y; i >= 0; i--)
			{
				if (Points[begin.X][i])
				{
					top = i;
				}
				else
				{
					break;
				}
			}

			var bottom = begin.Y;
			for (var i = begin.Y; i < Size.Height; i++)
			{
				if (Points[begin.X][i])
				{
					bottom = i;
				}
				else
				{
					break;
				}
			}

			var right = begin.X;
			for (var iX = begin.X; iX < Size.Width; iX++)
			{
				if (ContainsVertical(top, bottom, iX))
				{
					right = iX;
				}
				else
				{
					break;
				}
			}

			var left = begin.X;
			for (var iX = begin.X; iX >= 0; iX--)
			{
				if (ContainsVertical(top, bottom, iX))
				{
					right = iX;
				}
				else
				{
					break;
				}
			}

			return Rectangle.FromLTRB(left, top, right, bottom);
		}

		private bool ContainsVertical(int beginY, int endY, int x)
		{
			for (var iY = beginY; iY <= endY; iY++)
			{
				if (!Points[x][iY])
				{
					return false;
				}
			}

			return true;
		}
	}
}
