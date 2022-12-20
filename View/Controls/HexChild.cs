using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
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

		public HexChild(Game game, HexGrid owner, HexNodeVm source) : base(game)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Source = source ?? throw new ArgumentNullException(nameof(source));

			Source.PropertyChanged += SourcePropertyChanged;
		}

		public HexGrid Owner { get; }

		public HexNodeVm Source { get; }

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
				}

				return innerControl;
			}
		}

		private void SourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(HexNodeVm.State):
					UpdateState();
					break;
			}
		}

		private void UpdateState()
		{
			InnerControl.Color = Source.State switch
			{
				NodeState.Node => default,
				NodeState.Current => Color.Yellow,
				NodeState.Planned => Color.Green,
				_ => throw EnumNotSupportedException.Create(Source.State),
			};
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

			var position = Source.Position;
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

			return Utils.GetRegularPoligon(size, HEX_VERTICES_COUNT, offsetAngle)
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
			var game = ((Game1)Game);

			var mouseContains = Contains(game.MousePosition().ToVector2());

			if (mouseContains)
			{
				Color = Color.Blue;
			}
			else
			{
				Color = Color.Red;
			}

			if (mouseContains && game.MouseButtonOnPress())
			{
				// TODO Реализовать выбор текущей фигуры
			}

			base.Update(gameTime);
		}

		public bool Contains(Vector2 point) => Bounds.Contains(point.X, point.Y) && Utils.Contains(Vertices, point);

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				InnerControl.Dispose();
				Source.PropertyChanged -= SourcePropertyChanged;
			}

			base.Dispose(disposing);
		}
	}
}
