using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Common.Exceptions;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
	public class Text : Control
	{
		public Text(Game game, RectPlot plot) : base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
		}

		private RectPlot Plot { get; }

		private string? content;

		public string? Content
		{
			get => content;
			set
			{
				if (Content != value)
				{
					content = value;
					contentSize = null;
				}
			}
		}

		private SpriteFont? font;

		public SpriteFont? Font
		{
			get => font;
			set
			{
				if (Font != value)
				{
					font = value;
					contentSize = null;
				}
			}
		}

		private Vector2? contentSize;

		public Vector2 ContentSize => contentSize ??= Font?.MeasureString(Content) ?? Vector2.Zero;

		public Color Color { get; set; }

		private HorizontalAlign horizontalAlign;

		public HorizontalAlign HorizontalAlign
		{
			get => horizontalAlign;
			set
			{
				if (HorizontalAlign != value)
				{
					horizontalAlign = value;
					contentPosition = null;
				}
			}
		}

		private VerticalAlign verticalAlign;

		public VerticalAlign VerticalAlign
		{
			get => verticalAlign;
			set
			{
				if (VerticalAlign != value)
				{
					verticalAlign = value;
					contentPosition = null;
				}
			}
		}

		private Vector2? contentPosition;

		public Vector2 ContentPosition
		{
			get
			{
				if (contentPosition is null)
				{
					var x = HorizontalAlign switch
					{
						HorizontalAlign.Left => Plot.Bounds.Left,
						HorizontalAlign.Center => Plot.Bounds.Left + (Plot.Bounds.Width - ContentSize.X) / 2,
						HorizontalAlign.Right => Plot.Bounds.Right - ContentSize.X,
						_ => throw EnumNotSupportedException.Create(HorizontalAlign),
					};

					var y = VerticalAlign switch
					{
						VerticalAlign.Top => Plot.Bounds.Top,
						VerticalAlign.Center => Plot.Bounds.Top + (Plot.Bounds.Height - ContentSize.Y) / 2,
						VerticalAlign.Bottom => Plot.Bounds.Bottom - ContentSize.Y,
						_ => throw EnumNotSupportedException.Create(VerticalAlign),
					};

					contentPosition = new(x, y);
				}

				return contentPosition.Value;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (!string.IsNullOrWhiteSpace(Content) && Font is { })
			{
				((Game1)Game).SpriteBatch.DrawString(
					Font,
					Content,
					ContentPosition,
					Color
				);
			}
		}
	}
}
