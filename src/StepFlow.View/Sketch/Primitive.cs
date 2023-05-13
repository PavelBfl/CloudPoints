using System;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Sketch
{
	public class Primitive
	{
		public Primitive(Game game)
		{
			Game = game ?? throw new ArgumentNullException(nameof(game));
			Childs = new ChildsCollection(this);
		}

		public Game Game { get; }

		private Primitive? owner;

		public Primitive? Owner
		{
			get => owner;
			internal set
			{
				if (Owner != value)
				{
					owner = value;
				}
			}
		}

		public ChildsCollection Childs { get; }

		public bool Visible { get; set; } = true;

		public virtual void Draw(GameTime gameTime)
		{
		}

		public virtual void Update(GameTime gameTime)
		{
		}

		public virtual void Free()
		{
			foreach (var child in Childs)
			{
				child.Free();
			}
		}
	}
}
