using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StepFlow.ViewModel;

namespace StepFlow.View.Controlers
{
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
							InnerHex.Color = Microsoft.Xna.Framework.Color.Green;
							break;
						case NodeState.Planned:
							InnerHex.Visible = true;
							InnerHex.Color = Microsoft.Xna.Framework.Color.Yellow;
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
			Game.SpriteBatch.DrawPolygon(Vertices, Source.IsSelected ? Microsoft.Xna.Framework.Color.Blue : Color);
			base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			Source.IsSelected = Contains(Game.MousePosition());
			if (Source.IsSelected && Game.MouseButtonOnPress())
			{
				
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
