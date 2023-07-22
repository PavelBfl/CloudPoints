using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Common.Exceptions;
using StepFlow.View.Services;

namespace StepFlow.View.Controls
{
	public class Text : LayoutControl
	{
		public Text(IServiceProvider serviceProvider) : base(serviceProvider) => Drawer = ServiceProvider.GetRequiredService<IDrawer>();

		private IDrawer Drawer { get; }

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
						HorizontalAlign.Left => Place.Left,
						HorizontalAlign.Center => Place.Left + (Place.Width - ContentSize.X) / 2,
						HorizontalAlign.Right => Place.Right - ContentSize.X,
						_ => throw EnumNotSupportedException.Create(HorizontalAlign),
					};

					var y = VerticalAlign switch
					{
						VerticalAlign.Top => Place.Top,
						VerticalAlign.Center => Place.Top + (Place.Height - ContentSize.Y) / 2,
						VerticalAlign.Bottom => Place.Bottom - ContentSize.Y,
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
				Drawer.SpriteBatch.DrawString(
					Font,
					Content,
					ContentPosition,
					Color
				);
			}
		}
	}
}
