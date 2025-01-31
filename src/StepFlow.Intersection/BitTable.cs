﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Intersection
{
	public sealed class BitTable
	{
		public static BitTable CreateCircle(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}

			const int SCALE = 100;
			const int OFFSET = 45;

			var radius = (size * SCALE) / 2;
			var sqrRadius = radius * radius;

			var result = new BitTable(new Size(size, size));
			for (var iX = 0; iX < size; iX++)
			{
				for (var iY = 0; iY < size; iY++)
				{
					var rect = new Rectangle(
						iX * SCALE - radius + OFFSET,
						iY * SCALE - radius + OFFSET,
						SCALE - OFFSET * 2,
						SCALE - OFFSET * 2
					);

					if (Contains(rect.Left, rect.Top) ||
						Contains(rect.Right, rect.Top) ||
						Contains(rect.Right, rect.Bottom) ||
						Contains(rect.Left, rect.Bottom)
					)
					{
						result[iX, iY] = true;
					}
				}
			}

			return result;

			bool Contains(int x, int y)
			{
				var sqrDistance = (x * x) + (y * y);
				return sqrDistance <= sqrRadius;
			}
		}

		public BitTable(Size size)
		{
			Points = new BitArray[size.Width];
			for (var i = 0; i < Points.Length; i++)
			{
				Points[i] = new BitArray(size.Height);
			}
		}

		public bool this[int x, int y]
		{
			get => Points[x][y];
			set => Points[x][y] = value;
		}

		private BitArray[] Points { get; set; } = Array.Empty<BitArray>();

		public Size Size
		{
			get => new Size(Points.Length, Points.Length > 0 ? Points[0].Length : 0);
			private set
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

		public IEnumerable<Rectangle> CreateBordered()
		{
			var result = new List<Rectangle>();

			for (var iX = 0; iX < Size.Width; iX++)
			{
				for (var iY = 0; iY < Size.Height; iY++)
				{
					var prevLeft = new Point(iX - 1, iY);
					var prevTop = new Point(iX, iY - 1);

					if (
						(!GetSafeFlag(prevLeft) || result.Any(x => x.Contains(prevLeft))) &&
						(!GetSafeFlag(prevTop) || result.Any(x => x.Contains(prevTop))) &&
						GetSafeFlag(new Point(iX, iY))
					)
					{
						result.Add(ExpandFromLeft(new Point(iX, iY)));
					}
				}
			}

			return result;
		}

		private Rectangle ExpandFromLeft(Point begin)
		{
			var top = begin.Y;
			for (var iY = begin.Y; iY >= 0; iY--)
			{
				if (Points[begin.X][iY])
				{
					top = iY;
				}
				else
				{
					break;
				}
			}

			var bottom = begin.Y;
			for (var iY = begin.Y; iY < Size.Height; iY++)
			{
				if (Points[begin.X][iY])
				{
					bottom = iY;
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
					left = iX;
				}
				else
				{
					break;
				}
			}

			return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
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
