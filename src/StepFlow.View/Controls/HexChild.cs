using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.ViewModel;

namespace StepFlow.View.Controls
{
	public class HexChild : PolygonBase
	{
		private const int HEX_VERTICES_COUNT = 6;
		private const int POINTY_SPACING = 3;
		private const int FLAT_SPACING = 2;
		private const float FLAT_ANGLE_OFFSET = 0;
		private const float POINTY_ANGLE_OFFSET = MathF.Tau / (HEX_VERTICES_COUNT * 2);
		private const int MARKED_THICKNESS = 5;
		private const int UNMARKED_THICKNESS = 1;

		public HexChild(Game game, HexGrid owner, NodeVm? source = null) : base(game)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Source = source;

			SourceChanged();
		}

		public HexGrid Owner { get; }

		private NodeVm? source;

		public NodeVm? Source
		{
			get => source;
			set
			{
				if (Source != value)
				{
					NotifyPropertyExtensions.TryUnsubscribe(Source, SourcePropertyChanged);

					source = value;

					NotifyPropertyExtensions.TrySubscribe(Source, SourcePropertyChanged);

					SourceChanged();
				}
			}
		}

		private Polygon? innerControl;
		private Polygon InnerControl
		{
			get
			{
				if (innerControl is null)
				{
					innerControl = new Polygon(Game)
					{
						CustomVertices = CreateVertices(Owner.Size * 0.9f).ToList(),
					};

					Game.Components.Add(innerControl);

					UpdateState();
					UpdateIsMark();
				}

				return innerControl;
			}
		}

		private void SourceChanged()
		{
			UpdateState();
			UpdateIsMark();

			var visible = Source is { };
			Visible = visible;
			InnerControl.Visible = visible;
		}

		private void SourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(NodeVm.State):
					UpdateState();
					break;
				case nameof(NodeVm.IsMark):
					UpdateIsMark();
					break;
			}
		}

		private void UpdateIsMark()
		{
			if (Source is { })
			{
				InnerControl.Thickness = Source.IsMark ? MARKED_THICKNESS : UNMARKED_THICKNESS;
			}
			else
			{
				InnerControl.Thickness = 0;
			}
		}

		private void UpdateState()
		{
			if (Source is { })
			{
				if (Source.State.ContainsKey(NodeState.Current))
				{
					InnerControl.Color = Color.Yellow;
				}
				else if (Source.State.ContainsKey(NodeState.Planned))
				{
					InnerControl.Color = Color.Green;
				}
				else
				{
					InnerControl.Color = default;
				}
			}
			else
			{
				InnerControl.Color = default;
			}
		}

		private System.Drawing.RectangleF? bounds;

		public System.Drawing.RectangleF Bounds
		{
			get
			{
				if (bounds is null)
				{
					var min = new Vector2(float.MaxValue);
					var max = new Vector2(float.MinValue);

					foreach (var vertice in Vertices)
					{
						min.X = MathF.Min(min.X, vertice.X);
						min.Y = MathF.Min(min.Y, vertice.Y);
						max.X = MathF.Max(max.X, vertice.X);
						max.Y = MathF.Max(max.Y, vertice.Y);
					}

					bounds = new System.Drawing.RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
				}

				return bounds.Value;
			}
		}

		private Vector2[]? vertices;
		public override IReadOnlyList<Vector2> Vertices => vertices ??= CreateVertices(Owner.Size).ToArray();

		private static bool IsOdd(int value) => value % 2 == 1;

		private IEnumerable<Vector2> CreateVertices(float size)
		{
			var offsetAngle = Owner.Orientation switch
			{
				HexOrientation.Flat => FLAT_ANGLE_OFFSET,
				HexOrientation.Pointy => POINTY_ANGLE_OFFSET,
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var position = Source?.Position ?? System.Drawing.Point.Empty;
			var cellPosition = Owner.Orientation switch
			{
				HexOrientation.Flat => new Point(
					position.X * POINTY_SPACING,
					position.Y * FLAT_SPACING + (IsOdd(position.X) == Owner.OffsetOdd ? 1 : 0)
				),
				HexOrientation.Pointy => new Point(
					position.X * FLAT_SPACING + (IsOdd(position.Y) == Owner.OffsetOdd ? 1 : 0),
					position.Y * POINTY_SPACING
				),
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var cellOffset = Owner.GetPosition(cellPosition);

			return Utils.GetRegularPolygon(size, HEX_VERTICES_COUNT, offsetAngle)
				.Select(x => x + cellOffset);
		}

		public void Clear()
		{
			bounds = null;
			vertices = null;

			InnerControl.CustomVertices = CreateVertices(Owner.Size * 0.9f).ToList();
		}

		public override void Update(GameTime gameTime)
		{
			if (Source is null || Source.Owner is null)
			{
				return;
			}

			var game = ((Game1)Game);

			var mouseContains = Contains(game.MousePosition().ToVector2());

			Color = mouseContains ? Color.Blue : Color.Red;

			if (mouseContains && game.MouseButtonOnPress())
			{
				if (game.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
				{
					var last = Source.CreateSimple();
					Source.Owner.Current = last;
				}
				else if (game.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
				{
					foreach (var piece in Source.Owner.Pieces.Where(x => x.IsMark))
					{
						piece.MoveTo(Source);
					}
				}
				else
				{
					Source.Owner.Current = Source.Owner.Pieces.FirstOrDefault(x => x.Current == Source);
				}
			}

			base.Update(gameTime);
		}

		public bool Contains(Vector2 point) => Bounds.Contains(point.X, point.Y) && Utils.Contains(Vertices, point);

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				InnerControl.Dispose();
				Source = null;
			}

			base.Dispose(disposing);
		}
	}
}
