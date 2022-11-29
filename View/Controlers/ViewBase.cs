using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controlers
{
	public class ViewBase : DrawableGameComponent, INotifyPropertyChanged
	{
		public ViewBase(Game1 game)
			: base(game)
		{
		}

		public new Game1 Game => (Game1)base.Game;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
