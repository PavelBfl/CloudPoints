using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public class PointsCollection : Points
	{
		public PointsCollection(Playground owner) : base(owner)
		{
		}

		private HashSet<Point> Points { get; } = new HashSet<Point>();

		public bool Add(Point point)
		{
			var result = Points.Add(point);
			if (result)
			{
				bounds = null;
			}

			return result;
		}

		public bool Remove(Point point)
		{
			var result = Points.Remove(point);
			if (result)
			{
				bounds = null;
			}

			return result;
		}

		public override bool Contains(Point point) => Points.Contains(point);

		public override void Offset(Course course)
		{
			var offset = course.ToOffset();

			var newPoints = new Point[Points.Count];
			var index = 0;
			foreach (var point in Points)
			{
				point.Offset(offset);
				newPoints[index] = point;
				index++;
			}

			Points.Clear();
			Points.UnionWith(newPoints);
		}

		public override IEnumerator<Point> GetEnumerator() => Points.GetEnumerator();

		private Rectangle? bounds;

		public override Rectangle Bounds
		{
			get
			{
				if (bounds is null)
				{
					if (Points.Any())
					{
						var left = int.MaxValue;
						var right = int.MinValue;
						var top = int.MaxValue;
						var bottom = int.MinValue;

						foreach (var point in Points)
						{
							left = Math.Min(left, point.X);
							right = Math.Max(right, point.X);
							top = Math.Min(top, point.Y);
							bottom = Math.Max(bottom, point.Y);
						}
						bounds = Rectangle.FromLTRB(left, top, right, bottom);
					}
					else
					{
						bounds = Rectangle.Empty;
					}
				}

				return bounds.Value;
			}
		}
	}
}
