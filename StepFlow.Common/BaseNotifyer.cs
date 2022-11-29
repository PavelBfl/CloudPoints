using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StepFlow.Common
{
	public class BaseNotifyer : INotifyPropertyChanged
	{
		protected void SetValue<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
		{
			this.SetValue(ref field, newValue, PropertyChanged, propertyName);
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
			=> OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}
}
