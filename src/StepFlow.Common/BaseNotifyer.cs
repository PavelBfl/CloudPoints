using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StepFlow.Common
{
	public class BaseNotifyer : INotifyPropertyChanged, INotifyPropertyChanging
	{
		protected void SetValue<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				OnPropertyChanging(propertyName);
				field = newValue;
				OnPropertyChanged(propertyName);
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public event PropertyChangingEventHandler? PropertyChanging;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

		protected virtual void OnPropertyChanging(PropertyChangingEventArgs e) => PropertyChanging?.Invoke(this, e);

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
			=> OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

		protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
			=> OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
	}
}
