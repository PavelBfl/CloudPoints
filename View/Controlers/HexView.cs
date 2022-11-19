using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controlers
{
	public class HexView : PolygonBase
	{
		public HexView(Game1 game)
			: base(game)
		{
		}

		private Vector2 location = Vector2.Zero;
		public Vector2 Location
		{
			get => location;
			set
			{
				if (Location != value)
				{
					location = value;
					ClearCache();
					OnPropertyChanged();
				}
			}
		}

		private float size = 0;
		public float Size
		{
			get => size;
			set
			{
				if (Size != value)
				{
					size = value;
					ClearCache();
					OnPropertyChanged();
				}
			}
		}

		private void ClearCache()
		{
			vertices = null;
		}

		private Vector2[]? vertices = null;
		public Vector2[] Vertices => vertices ??= Utils.GetRegularPoligon(Size, 6, 0).Select(x => x + Location).ToArray();

		public override void Draw(GameTime gameTime)
		{
			Game.SpriteBatch.DrawPolygon(Vertices, Color);

			base.Draw(gameTime);
		}

		public bool Contains(Microsoft.Xna.Framework.Point point)
		{
			var result = false;
			var prevIndex = Vertices.Length - 1;
			for (int i = 0; i < Vertices.Length; i++)
			{
				var prevPoint = Vertices[prevIndex];
				var currentPoint = Vertices[i];
				if (currentPoint.Y < point.Y && prevPoint.Y >= point.Y || prevPoint.Y < point.Y && currentPoint.Y >= point.Y)
				{
					if (currentPoint.X + (point.Y - currentPoint.Y) / (prevPoint.Y - currentPoint.Y) * (prevPoint.X - currentPoint.X) < point.X)
					{
						result = !result;
					}
				}
				prevIndex = i;
			}
			return result;
		}

		public override IReadOnlyList<Vector2> GetVertices() => Vertices;
	}
}
