using System;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Sketch
{
	public class Primitive
	{
		public Primitive(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			Childs = new ChildsCollection(this);
		}

		public IServiceProvider ServiceProvider { get; }

		private Primitive? owner;

		public Primitive? Owner
		{
			get => owner;
			internal set
			{
				if (Owner != value)
				{
					owner = value;

					OnOwnerChange();
				}
			}
		}

		protected virtual void OnOwnerChange()
		{
		}

		public ChildsCollection Childs { get; }

		public bool Visible { get; set; } = true;

		public virtual void Draw(GameTime gameTime)
		{
		}

		public bool Enable { get; set; } = true;

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
