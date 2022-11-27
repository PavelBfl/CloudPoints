using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controlers
{
	public class ViewBase : DrawableGameComponent, INotifyPropertyChanged
	{
		public ViewBase(Game1 game)
			: base(game)
		{
		}

		public new Game1 Game => (Game1)base.Game;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public interface IPlace : INotifyPropertyChanged
	{
		System.Drawing.RectangleF Place { get; }
	}

	public class ViewLayout : ViewBase
	{
		public ViewLayout(Game1 game)
					: base(game)
		{
		}

		private IPlace? placeOwner;
		public IPlace? PlaceOwner
		{
			get => placeOwner;
			set
			{
				if (PlaceOwner != value)
				{
					if (PlaceOwner is not null)
					{
						PlaceOwner.PropertyChanged -= PlaceOwnerPropertyChanged;
					}

					placeOwner = value;

					if (PlaceOwner is not null)
					{
						PlaceOwner.PropertyChanged += PlaceOwnerPropertyChanged;
					}
				}
			}
		}

		private void PlaceOwnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			bound = null;
		}

		private System.Drawing.RectangleF? bound;

		public System.Drawing.RectangleF Bound => bound ??= CreateBound();

		private Margin margin;
		public Margin Margin
		{
			get => margin;
			set
			{
				margin = value;
				bound = null;
			}
		}

		private System.Drawing.SizeF size;
		public System.Drawing.SizeF Size
		{
			get => size;
			set
			{
				size = value;
				bound = null;
			}
		}

		private System.Drawing.RectangleF CreateBound()
		{
			if (PlaceOwner?.Place is { } place)
			{
				var horizontal = GetAlign(place.Left, place.Right, Margin.Left, Margin.Right, Size.Width);
				var vertical = GetAlign(place.Top, place.Bottom, Margin.Top, Margin.Bottom, Size.Height);

				return new System.Drawing.RectangleF(
					x: horizontal.Min,
					y: vertical.Min,
					width: horizontal.Max - horizontal.Min,
					height: vertical.Max - vertical.Min
				);
			}
			else
			{
				return new System.Drawing.RectangleF();
			}
		}

		private static (float Min, float Max) GetAlign(float globalMin, float globalMax, float? min, float? max, float length)
		{
			return (min, max) switch
			{
				(float minValue, float maxValue) => (globalMin + minValue, globalMax - maxValue),
				(float minValue, null) => (globalMin + minValue, globalMin + minValue + length),
				(null, float maxValue) => (globalMax - length - maxValue, globalMax - maxValue),
				(null, null) => (globalMin + (globalMax - globalMin - length) / 2, globalMax - (globalMax - globalMin - length) / 2)
			};
		}
	}

	public class GridLayout : ViewLayout
	{
		public GridLayout(Game1 game)
			: base(game)
		{
			Columns = new Lines(this);
			Rows = new Lines(this);
		}

		public IList<CellSize> Columns { get; }

		public IList<CellSize> Rows { get; }

		private Dictionary<ViewLayout, CellPlace> Childs { get; } = new();

		public void Add(ViewLayout child, CellPosition position)
		{
			var cellPlace = new CellPlace(System.Drawing.RectangleF.Empty)
			{
				Position = position,
			};
			child.PlaceOwner = cellPlace;

			Childs.Add(child, cellPlace);
			Refresh();
		}

		private void Refresh()
		{
			var columnsPixels = ToInstance(Columns, Bound.X, Bound.Width);
			var rowsPixels = ToInstance(Rows, Bound.Y, Bound.Height);

			foreach (var place in Childs.Values)
			{
				var position = place.Position;
				place.Place = new System.Drawing.RectangleF(
					x: columnsPixels[position.Column].Position,
					y: rowsPixels[position.Row].Position,
					width: columnsPixels.Skip(position.Column).Take(position.ColumnSpan).Select(x => x.Length).Sum(),
					height: rowsPixels.Skip(position.Row).Take(position.RowSpan).Select(x => x.Length).Sum()
				);
			}
		}

		private static Range[] ToInstance(IList<CellSize> items, float positionOffset, float size)
		{
			var result = new Range[items.Count];
			for (var i = 0; i < items.Count; i++)
			{
				var cell = items[i];
				var instance = cell.Measure switch
				{
					UnitMeasure.Pixels => new Range(positionOffset, cell.Value),
					UnitMeasure.Ptc => new Range(positionOffset, cell.Value * size),
					_ => throw new InvalidOperationException(),
				};

				positionOffset += instance.Length;
				result[i] = instance;
			}

			return result;
		}

		private readonly record struct Range(float Position, float Length);

		private sealed class CellPlace : IPlace
		{
			public CellPlace(System.Drawing.RectangleF place)
			{
				Place = place;
			}

			private System.Drawing.RectangleF place;
			public System.Drawing.RectangleF Place
			{
				get => place;
				set
				{
					if (Place != value)
					{
						place = value;
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Place)));
					}
				}
			}

			public CellPosition Position { get; set; }

			public event PropertyChangedEventHandler? PropertyChanged;
		}

		private sealed class Lines : IList<CellSize>, IReadOnlyList<CellSize>
		{
			public Lines(GridLayout owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public CellSize this[int index] { get => Items[index]; set => Items[index] = value; }

			public int Count => Items.Count;

			public bool IsReadOnly => false;

			private List<CellSize> Items { get; } = new();

			private GridLayout Owner { get; }

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

	public struct CellSize
	{
		public CellSize(float value, UnitMeasure measure)
		{
			Value = value;
			Measure = measure;
		}

		public float Value { get; set; }

		public UnitMeasure Measure { get; set; }
	}

	public enum UnitMeasure
	{
		Ptc,
		Pixels,
	}

	public struct CellPosition
	{
		public CellPosition(int column, int row, int columnSpan, int rowSpan)
		{
			Column = column >= 0 ? column : throw new ArgumentOutOfRangeException(nameof(column));
			Row = row >= 0 ? row : throw new ArgumentOutOfRangeException(nameof(row));
			ColumnSpan = columnSpan >= 1 ? columnSpan : throw new ArgumentOutOfRangeException(nameof(columnSpan));
			RowSpan = rowSpan >= 1 ? rowSpan : throw new ArgumentOutOfRangeException(nameof(rowSpan));
		}

		public CellPosition(int column, int row)
			: this(column, row, 1, 1)
		{
		}

		public int Column { get; set; }

		public int Row { get; set; }

		public int ColumnSpan { get; set; }

		public int RowSpan { get; set; }
	}

	public struct Margin
	{
		public Margin(float all)
			: this(all, all, all, all)
		{
		}

		public Margin(float? left, float? right, float? top, float? bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		public float? Left { get; set; }
		public float? Right { get; set; }
		public float? Top { get; set; }
		public float? Bottom { get; set; }
	}
}
