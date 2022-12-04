using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
			ChildsInner = new GridChildsCollection(this);
		}

		public IList<CellSize> Columns { get; }

		public IList<CellSize> Rows { get; }

		private GridChildsCollection ChildsInner { get; }

		public IGridChildsCollection Childs => ChildsInner;

		// TODO Can be private
		public void Refresh()
		{
			var columnsPixels = ToInstance(Columns, Bounds.X, Bounds.Width);
			var rowsPixels = ToInstance(Rows, Bounds.Y, Bounds.Height);

			foreach (var place in Childs)
			{
				var position = place.Position;
				place.Child.OwnerBounds = new RectangleF(
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

		private sealed class CellPlot : ICellPlot
		{
			public CellPlot(GridPlot owner, SubPlotRect child)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Child = child ?? throw new ArgumentNullException(nameof(child));
			}

			private GridPlot Owner { get; }

			private CellPosition position;

			public CellPosition Position
			{
				get => position;
				set
				{
					if (Position != value)
					{
						position = value;
						Owner.Refresh();
					}
				}
			}

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

		private class GridChildsCollection : ObservableCollection<CellPlot>, IGridChildsCollection
		{
			public GridChildsCollection(GridPlot owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			ICellPlot IReadOnlyList<ICellPlot>.this[int index] => this[index];

			private GridPlot Owner { get; }

			public void Add(SubPlotRect plot, CellPosition position)
			{
				Add(new CellPlot(Owner, plot)
				{
					Position = position
				});
			}

			public void Remove(SubPlotRect plot)
			{
				int? findIndex = null;
				for (var i = 0; i < Count; i++)
				{
					if (EqualityComparer<SubPlotRect>.Default.Equals(plot, this[i].Child))
					{
						findIndex = i;
						break;
					}
				}

				if (findIndex is { } instanceIndex)
				{
					RemoveAt(instanceIndex);
				}
			}

			IEnumerator<ICellPlot> IEnumerable<ICellPlot>.GetEnumerator() => GetEnumerator();
		}
	}

	public interface ICellPlot
	{
		CellPosition Position { get; }

		SubPlotRect Child { get; }
	}

	public interface IGridChildsCollection : IReadOnlyList<ICellPlot>, INotifyCollectionChanged
	{
		void Add(SubPlotRect plot, CellPosition position);
		void Remove(SubPlotRect plot);
	}
}
