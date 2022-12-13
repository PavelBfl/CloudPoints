using System;
using System.Collections.Generic;
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
		private float PointyAngleOffset { get; } = MathF.Tau / (HEX_VERTICES_COUNT * 2);

		public HexChild(Game game, HexGrid owner, HexNodeVm source) : base(game)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public HexGrid Owner { get; }
		public HexNodeVm Source { get; }

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

			return Utils.GetRegularPoligon(Owner.Size, HEX_VERTICES_COUNT, offsetAngle)
				.Select(x => x + cellOffset)
				.ToArray();
		}

		public void Clear() => vertices = null;
	}
}
