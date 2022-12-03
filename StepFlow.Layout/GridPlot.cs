using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Layout
{
	public class GridPlot : SubPlotRect
	{
		public GridPlot()
		{
			Columns = new Lines(this);
			Rows = new Lines(this);
		}

		public IList<CellSize> Columns { get; }

		public IList<CellSize> Rows { get; }

		private Dictionary<SubPlotRect, CellPlot> Childs { get; } = new Dictionary<SubPlotRect, CellPlot>();

		public void Add(SubPlotRect child, CellPosition position)
		{
			if (child is null)
			{
				throw new ArgumentNullException(nameof(child));
			}

			var cellPlace = new CellPlot(RectangleF.Empty, child)
			{
				Position = position,
			};

			Childs.Add(child, cellPlace);
			Refresh();
		}

		private void Refresh()
		{
			var columnsPixels = ToInstance(Columns, Bounds.X, Bounds.Width);
			var rowsPixels = ToInstance(Rows, Bounds.Y, Bounds.Height);

			foreach (var place in Childs.Values)
			{
				var position = place.Position;
				place.Bounds = new RectangleF(
					x: columnsPixels[position.Column].Position,
					y: rowsPixels[position.Row].Position,
					width: columnsPixels.Skip(position.Column).Take(position.ColumnSpan).Select(x => x.Length).Sum(),
					height: rowsPixels.Skip(position.Row).Take(position.RowSpan).Select(x => x.Length).Sum()
				);
			}
		}

		private static Range[] ToInstance(IList<CellSize> items, float positionOffset, float size)
		{
			var allowSize = size - items.Where(x => x.Measure == UnitMeasure.Pixels).Sum(x => x.Value);

			var result = new Range[items.Count];
			for (var i = 0; i < items.Count; i++)
			{
				var cell = items[i];
				var instance = cell.Measure switch
				{
					UnitMeasure.Pixels => new Range(positionOffset, cell.Value),
					UnitMeasure.Ptc => new Range(positionOffset, cell.Value * allowSize),
					_ => throw new InvalidOperationException(),
				};

				positionOffset += instance.Length;
				result[i] = instance;
			}

			return result;
		}

		private struct Range
		{
			public Range(float position, float length)
			{
				Position = position;
				Length = length;
			}

			public float Position { get; set; }
			public float Length { get; set; }
		}

		private sealed class CellPlot
		{
			public CellPlot(RectangleF bounds, SubPlotRect child)
			{
				Bounds = bounds;
				Child = child ?? throw new ArgumentNullException(nameof(child));
			}

			private RectangleF bounds;
			public RectangleF Bounds
			{
				get => bounds;
				set
				{
					if (Bounds != value)
					{
						bounds = value;
						Child.OwnerBounds = Bounds;
					}
				}
			}

			public CellPosition Position { get; set; }

			public SubPlotRect Child { get; }
		}

		private sealed class Lines : IList<CellSize>, IReadOnlyList<CellSize>
		{
			public Lines(GridPlot owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public CellSize this[int index] { get => Items[index]; set => Items[index] = value; }

			public int Count => Items.Count;

			public bool IsReadOnly => false;

			private List<CellSize> Items { get; } = new List<CellSize>();

			private GridPlot Owner { get; }

			public void Add(CellSize item)
			{
				Items.Add(item);
				Owner.Refresh();
			}

			public void Clear()
			{
				Items.Clear();
				Owner.Refresh();
			}

			public bool Contains(CellSize item) => Items.Contains(item);

			public void CopyTo(CellSize[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

			public IEnumerator<CellSize> GetEnumerator() => Items.GetEnumerator();

			public int IndexOf(CellSize item) => Items.IndexOf(item);

			public void Insert(int index, CellSize item)
			{
				Items.Insert(index, item);
				Owner.Refresh();
			}

			public bool Remove(CellSize item)
			{
				var result = Items.Remove(item);
				if (result)
				{
					Owner.Refresh();
				}

				return result;
			}

			public void RemoveAt(int index)
			{
				Items.RemoveAt(index);
				Owner.Refresh();
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
