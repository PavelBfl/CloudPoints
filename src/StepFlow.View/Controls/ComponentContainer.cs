using System;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class ComponentContainer : GameComponent
	{
		public ComponentContainer(Game game) : base(game)
		{
		}

		private GameComponentCollection Childs { get; } = new GameComponentCollection();

		protected T Add<T>(T component)
			where T : class, IGameComponent
		{
			if (component is null)
			{
				throw new ArgumentNullException(nameof(component));
			}

			Game.Components.Add(component);

			return component;
		}


		protected override void Dispose(bool disposing)
		{
			foreach (var child in Childs)
			{
				Game.Components.Remove(child);
			}

			Game.Components.Remove(this);

			base.Dispose(disposing);
		}
	}
}
