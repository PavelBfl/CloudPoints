using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StepFlow.View.Services
{
	public interface IDrawer
	{
		SpriteBatch SpriteBatch { get; }

		void Line(Vector2 start, Vector2 end, Color color, float thickness = 2);

		void Polygon(IReadOnlyList<Vector2> vertices, Color color, float thickness = 2);
	}
}
