using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StepFlow.ViewModel
{
	public class BaseVm : INotifyPropertyChanged
	{
		protected void SetValue<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				field = newValue;
				OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
	}
}
