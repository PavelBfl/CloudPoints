using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StepFlow.ViewModel;

namespace StepFlow.View.Controlers
{
	public class HexView : ViewBase
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

		public Color Color { get; set; } = Color.Black;

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

	public class HexNodeView : HexView
	{
		public HexNodeView(Game1 game, HexNodeVm source)
			: base(game)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));

			InnerHex = new HexView(game)
			{
				Size = Size * InnerPtc,
				Location = Location,
			};
			Game.Components.Add(InnerHex);

			Source.PropertyChanged += SourcePropertyChanged;
		}


		public HexNodeVm Source { get; }

		public float InnerPtc { get; set; } = 0.8f;

		private HexView InnerHex { get; }

		private void SourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(HexNodeVm.State):
					switch (Source.State)
					{
						case NodeState.Current:
							InnerHex.Visible = true;
							InnerHex.Color = Color.Green;
							break;
						case NodeState.Planned:
							InnerHex.Visible = true;
							InnerHex.Color = Color.Yellow;
							break;
						default:
							InnerHex.Visible = false;
							break;
					}
					break;
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			switch (propertyName)
			{
				case nameof(Size):
					InnerHex.Size = Size * InnerPtc;
					break;
				case nameof(Location):
					InnerHex.Location = Location;
					break;
			}

			base.OnPropertyChanged(propertyName);
		}

		public override void Draw(GameTime gameTime)
		{
			Game.SpriteBatch.DrawPolygon(Vertices, Source.IsSelected ? Color.Blue : Color);
			base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			Source.IsSelected = Contains(Game.MousePosition());
			if (Source.IsSelected && Game.MouseButtonOnPress())
			{
				Source.State = NodeState.Planned;
			}

			base.Update(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Game.Components.Remove(InnerHex);

				Source.PropertyChanged -= SourcePropertyChanged;
			}

			base.Dispose(disposing);
		}
	}
}
