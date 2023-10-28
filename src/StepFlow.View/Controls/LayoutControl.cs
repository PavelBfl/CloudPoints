using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using StepFlow.View.Sketch;

namespace StepFlow.View.Controls
{
	public class LayoutControl : Primitive
	{
		public LayoutControl(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			Bounds = new Polygon(serviceProvider)
			{
				Vertices = Vertices,
			};
		}

		private Polygon Bounds { get; }

		private BoundsVertices vertices = new BoundsVertices();

		public IReadOnlyVertices Vertices => vertices;

		public Layout? Layout { get; set; }

		public override void Draw(GameTime gameTime)
		{
			vertices.Bounds = Layout?.Place ?? RectangleF.Empty;

			Bounds.Draw(gameTime);
			base.Draw(gameTime);
		}
	}
}
