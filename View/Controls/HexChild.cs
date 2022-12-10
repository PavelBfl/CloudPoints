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
				HexOrientation.Flat => 0,
				HexOrientation.Pointy => MathF.Tau / (HEX_VERTICES_COUNT * 2),
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var lineOffset = Owner.OffsetOdd ? 1 : 0;

			var cellPosition = Owner.Orientation switch
			{
				HexOrientation.Flat => new Point(
					Position.X * 3,
					Position.Y * 2 + (IsOdd(Position.X) == Owner.OffsetOdd ? 1 : 0)
				),
				HexOrientation.Pointy => new Point(
					Position.X * 2 + (IsOdd(Position.Y) == Owner.OffsetOdd ? 1 : 0),
					Position.Y * 3
				),
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var cellOffset = new Vector2(
				cellPosition.X * Owner.CellWidth,
				cellPosition.Y * Owner.CellHeight
			) + new Vector2(20);

			return Utils.GetRegularPoligon(Owner.Size, HEX_VERTICES_COUNT, offsetAngle)
				.Select(x => x + cellOffset)
				.ToArray();
		}

		public void Clear() => vertices = null;
	}
}
