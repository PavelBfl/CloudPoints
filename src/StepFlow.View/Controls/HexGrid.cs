//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using StepFlow.Common.Exceptions;
//using StepFlow.View.Services;
//using StepFlow.View.Sketch;

//namespace StepFlow.View.Controls
//{
//	public class HexGrid : LayoutControl, IReadOnlyDictionary<Point, ILayoutCanvas>
//	{
//		private static float BigRadiusToFlatRatio { get; } = MathF.Sqrt(3);
//		private static (float Pointy, float Flat, float CellPointy, float CellFlat) GetSize(float bigRadius)
//		{
//			var pointy = bigRadius * 2;
//			var flat = bigRadius * BigRadiusToFlatRatio;
//			return (pointy, flat, pointy / 4, flat / 2);
//		}

//		public HexGrid(IServiceProvider serviceProvider) : base(serviceProvider)
//		{
//		}

//		private HexOrientation orientation = HexOrientation.Flat;

//		public HexOrientation Orientation
//		{
//			get => orientation;
//			set
//			{
//				if (Orientation != value)
//				{
//					orientation = value;
//					hexSize = null;
//				}
//			}
//		}

//		private bool offsetOdd;

//		public bool OffsetOdd
//		{
//			get => offsetOdd;
//			set
//			{
//				if (OffsetOdd != value)
//				{
//					offsetOdd = value;
//					hexSize = null;
//				}
//			}
//		}

//		public float size = 1;

//		public float Size
//		{
//			get => size;
//			set
//			{
//				if (Size != value)
//				{
//					size = value;
//					hexSize = null;
//				}
//			}
//		}

//		private HexSize CreateHexSize()
//		{
//			float width;
//			float height;
//			float cellWidth;
//			float cellHeight;
//			switch (Orientation)
//			{
//				case HexOrientation.Flat:
//					(width, height, cellWidth, cellHeight) = GetSize(Size);
//					break;
//				case HexOrientation.Pointy:
//					(height, width, cellHeight, cellWidth) = GetSize(Size);
//					break;
//				default: throw EnumNotSupportedException.Create(Orientation);
//			}

//			return new(width, height, cellWidth, cellHeight);
//		}

//		private HexSize? hexSize;

//		public HexSize HexSize => hexSize ??= CreateHexSize();

//		public Vector2 GetPosition(Point cellPosition) => new Vector2(
//			cellPosition.X * HexSize.CellWidth + Place.Left + HexSize.Width / 2,
//			cellPosition.Y * HexSize.CellHeight + Place.Top + HexSize.Height / 2
//		);

//		private Dictionary<Point, HexCell> Cells { get; } = new();

//		public IEnumerable<Point> Keys => Cells.Keys;

//		public IEnumerable<ILayoutCanvas> Values => Cells.Values;

//		public int Count => Cells.Count;

//		public ILayoutCanvas this[Point key] => Cells[key];

//		public ILayoutCanvas Add(Point position)
//		{
//			var result = new HexCell(this, position);
//			Cells.Add(position, result);
//			return result;
//		}

//		public bool ContainsKey(Point key) => Cells.ContainsKey(key);

//		public bool TryGetValue(Point key, [MaybeNullWhen(false)] out ILayoutCanvas value)
//		{
//			if (Cells.TryGetValue(key, out var result))
//			{
//				value = result;
//				return true;
//			}
//			else
//			{
//				value = default;
//				return false;
//			}
//		}

//		public IEnumerator<KeyValuePair<Point, ILayoutCanvas>> GetEnumerator()
//			=> Cells.Select(x => new KeyValuePair<Point, ILayoutCanvas>(x.Key, x.Value)).GetEnumerator();

//		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

//		private sealed class HexCell : ILayoutCanvas
//		{
//			public HexCell(HexGrid owner, Point position)
//			{
//				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
//				Position = position;
//			}

//			public HexGrid Owner { get; }

//			public Point Position { get; }

//			private System.Drawing.RectangleF? place;

//			public System.Drawing.RectangleF Place
//			{
//				get
//				{
//					if (place is null)
//					{
//						var position = Owner.GetPosition(Position);
//						place = new(
//							position.X - Owner.Width / 2,
//							position.Y - Owner.Height / 2,
//							Owner.Width,
//							Owner.Height
//						);
//					}

//					return place.Value;
//				}
//			}

//			public void Refresh() => place = null;
//		}
//	}

//	public readonly struct HexSize
//	{
//		public HexSize(float width, float height, float cellWidth, float cellHeight)
//		{
//			Width = width;
//			Height = height;
//			CellWidth = cellWidth;
//			CellHeight = cellHeight;
//		}

//		public float Width { get; }

//		public float Height { get; }

//		public float CellWidth { get; }

//		public float CellHeight { get; }
//	}
//}
