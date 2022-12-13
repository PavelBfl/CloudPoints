using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;

namespace StepFlow.View.Controls
{
	public class HexChild : PolygonBase
	{
		private const int HEX_VERTICES_COUNT = 6;
		private const int POINTY_SPACING = 3;
		private const int FLAT_SPACING = 2;
		private const float FLAT_ANGLE_OFFSET = 0;
		private float PointyAngleOffset { get; } = MathF.Tau / (HEX_VERTICES_COUNT * 2);

		public HexChild(Game game, HexGrid owner, Point position) : base(game)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Position = position;
		}

		public HexGrid Owner { get; }
		public Point Position { get; }

		private Vector2[]? vertices;
		public override IReadOnlyList<Vector2> Vertices => vertices ??= CreateVertices();

		private static bool IsOdd(int value) => value % 2 == 1;

		private Vector2[] CreateVertices()
		{
			var offsetAngle = Owner.Orientation switch
			{
				HexOrientation.Flat => FLAT_ANGLE_OFFSET,
				HexOrientation.Pointy => PointyAngleOffset,
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var cellPosition = Owner.Orientation switch
			{
				HexOrientation.Flat => new Point(
					Position.X * POINTY_SPACING,
					Position.Y * FLAT_SPACING + (IsOdd(Position.X) == Owner.OffsetOdd ? 1 : 0)
				),
				HexOrientation.Pointy => new Point(
					Position.X * FLAT_SPACING + (IsOdd(Position.Y) == Owner.OffsetOdd ? 1 : 0),
					Position.Y * POINTY_SPACING
				),
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};


			var cellOffset = Owner.GetPosition(cellPosition);

			return Utils.GetRegularPoligon(Owner.Size, HEX_VERTICES_COUNT, offsetAngle)
				.Select(x => x + cellOffset)
				.ToArray();
		}

		public void Clear() => vertices = null;
	}
}
