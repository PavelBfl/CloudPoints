using System;
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
				Thickness = 1,
				Color = Color.Red,
				Vertices = Vertices,
			};
		}

		private Polygon Bounds { get; }

		private BoundsVertices vertices = new BoundsVertices();

		public IReadOnlyVertices Vertices => vertices;

		public Color BoundsColor { get => Bounds.Color; set => Bounds.Color = value; }

		public Layout? Layout { get; set; }

		public override void Draw(GameTime gameTime)
		{
			vertices.Bounds = Layout?.Place ?? System.Drawing.RectangleF.Empty;

			Bounds.Draw(gameTime);
			base.Draw(gameTime);
		}
	}
}
