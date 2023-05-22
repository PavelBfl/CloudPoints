using System;
using System.ComponentModel;
using StepFlow.Common;

namespace StepFlow.ViewModel
{
	public class BaseVm : BaseNotifyer, IComponent
	{
		public ISite Site { get => Component.Site; set => Component.Site = value; }
		private IComponent Component { get; } = new Component();

		public event EventHandler Disposed
		{
			add
			{
				Component.Disposed += value;
			}

			remove
			{
				Component.Disposed -= value;
			}
		}

		public virtual void Dispose() => Component.Dispose();
	}
}
