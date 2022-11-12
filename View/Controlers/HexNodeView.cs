using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.ViewModel;

namespace StepFlow.View.Controlers
{
	public class ViewBase : DrawableGameComponent
	{
		public ViewBase(Game1 game)
			: base(game)
		{
		}

		public Game1 Game => (Game1)base.Game;
	}

	public class WrappaerView<T> : ViewBase
	{
		public WrappaerView(Game1 game, T source, bool checkSourceNull = false)
			: base(game)
		{
			if (checkSourceNull && source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			Source = source;
		}

		public T Source { get; }
	}

	public class HexNodeView : WrappaerView<HexNodeVm>
	{
		public HexNodeView(Game1 game, HexNodeVm source)
			: base(game, source, true)
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
				}
			}
		}

		public Color Color { get; set; } = Color.Black;

		public NodeState State { get; set; } = NodeState.Node;

		public bool IsSelected { get; set; } = false;

		private void ClearCache()
		{
			vertices = null;
			innerVertices = null;
		}

		private Vector2[]? vertices = null;
		private Vector2[] Vertices => vertices ??= Utils.GetRegularPoligon(Size, 6, 0).Select(x => x + Location).ToArray();

		private Vector2[]? innerVertices = null;
		private Vector2[] InnerVertices => innerVertices ??= Utils.GetRegularPoligon(Size * 0.8f, 6, 0).Select(x => x + Location).ToArray();

		public override void Draw(GameTime gameTime)
		{
			Game.SpriteBatch.DrawPolygon(Vertices, IsSelected ? Color.Blue : Color);

			Color? stateColor = null;
			switch (State)
			{
				case NodeState.Current:
					stateColor = Color.Green;
					break;
				case NodeState.Planned:
					stateColor = Color.Yellow;
					break;
			}

			if (stateColor is { } color)
			{
				Game.SpriteBatch.DrawPolygon(InnerVertices, color);
			}

			base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			Source.IsSelected = Contains(Game.MousePosition());

			base.Update(gameTime);
		}

		public bool Contains(Point point)
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
	}

	public enum NodeState
	{
		Node,
		Current,
		Planned,
	}

	public class MovementPieceView : WrappaerView<MovementPieceVm>
	{
		public MovementPieceView(Game1 game, MovementPieceVm source)
			: base(game, source, true)
		{
		}
	}
}
