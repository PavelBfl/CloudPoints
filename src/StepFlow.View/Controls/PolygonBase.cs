using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
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

		public Color Color { get; set; }

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
}
