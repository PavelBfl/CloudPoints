using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using StepFlow.Common.Exceptions;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View.Controls
{
	public abstract class PolygonBase : Primitive
	{
		public PolygonBase(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			Drawer = ServiceProvider.GetRequiredService<IDrawer>();
		}

		private IDrawer Drawer { get; }

		public Color Color { get; set; } = Color.Red;

		public float thickness = 1;

		public float Thickness
		{
			get => thickness;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				thickness = value;
			}
		}

		public abstract IReadOnlyVertices? GetVertices();

		public override void Draw(GameTime gameTime)
		{
			var vertices = GetVertices();
			if (vertices?.Any() ?? false)
			{
				Drawer.Polygon(vertices, Color, Thickness);
			}
		}
	}

	public class Polygon : PolygonBase
	{
		public Polygon(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		public IReadOnlyVertices? Vertices { get; set; }

		public override IReadOnlyVertices? GetVertices() => Vertices;
	}

	public class Hex : PolygonBase
	{
		private const int HEX_VERTICES_COUNT = 6;
		private const float FLAT_ANGLE_OFFSET = 0;
		private const float POINTY_ANGLE_OFFSET = MathF.Tau / (HEX_VERTICES_COUNT * 2);

		private IEnumerable<Vector2> CreateVertices(Vector2 center, HexOrientation orientation, float size)
		{
			var offsetAngle = orientation switch
			{
				HexOrientation.Flat => FLAT_ANGLE_OFFSET,
				HexOrientation.Pointy => POINTY_ANGLE_OFFSET,
				_ => throw EnumNotSupportedException.Create(orientation),
			};

			return Utils.GetRegularPolygon(size, HEX_VERTICES_COUNT, offsetAngle)
				.Select(x => x + center);
		}

		public Hex(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private Vector2 position;

		public Vector2 Position
		{
			get => position;
			set
			{
				if (Position != value)
				{
					position = value;

					Vertices = null;
				}
			}
		}

		private HexOrientation orientation;

		public HexOrientation Orientation
		{
			get => orientation;
			set
			{
				if (Orientation != value)
				{
					orientation = value;

					Vertices = null;
				}
			}
		}

		private float size;

		public float Size
		{
			get => size;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				if (Size != value)
				{
					size = value;

					Vertices = null;
				}
			}
		}

		private IReadOnlyVertices? Vertices { get; set; }

		public override IReadOnlyVertices? GetVertices()
			=> Vertices ??= new VerticesCollection(CreateVertices(Position, Orientation, Size));
	}
}
