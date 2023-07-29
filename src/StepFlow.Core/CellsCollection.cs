using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public class CellsCollection : Cells
	{
		public CellsCollection(Playground owner) : base(owner)
		{
		}

		private HashSet<Cell> Cells { get; } = new HashSet<Cell>();

		public Cell Add(Point current, Point? next = null)
		{
			var cell = new Cell(this)
			{
				Current = current,
				Next = next ?? current,
			};

			Cells.Add(cell);

			return cell;
		}

		public bool Remove(Cell cell)
		{
			if (cell is null)
			{
				throw new ArgumentNullException(nameof(cell));
			}

			var result = Cells.Remove(cell);
			if (result)
			{
				bounds = null;
			}

			return result;
		}

		public override IEnumerator<ICell> GetEnumerator() => Cells.GetEnumerator();

		private Rectangle? bounds;

		public override Rectangle Bounds
		{
			get
			{
				if (bounds is null)
				{
					if (Cells.Any())
					{
						var left = int.MaxValue;
						var right = int.MinValue;
						var top = int.MaxValue;
						var bottom = int.MinValue;

						foreach (var point in Cells)
						{
							left = Math.Min(left, point.Current.X);
							right = Math.Max(right, point.Current.X);
							top = Math.Min(top, point.Current.Y);
							bottom = Math.Max(bottom, point.Current.Y);
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

		public sealed class Cell : ICell
		{
			public Cell(CellsCollection owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			private CellsCollection Owner { get; }

			private Point current;

			public Point Current
			{
				get => current;
				set
				{
					if (Current != value)
					{
						current = value;
						Owner.bounds = null;
					}
				}
			}

			public Point Next { get; set; }
		}
	}
}
