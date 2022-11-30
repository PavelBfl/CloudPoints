using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StepFlow.Common
{
	public static class NotifyPropertyChangedExtentions
	{
		public static void SetValue<T>(
			this INotifyPropertyChanged notifyer,
			ref T field,
			T newValue,
			PropertyChangedEventHandler? propertyChanged,
			PropertyChangingEventHandler? propertyPropertyChanging = null,
			[CallerMemberName] string? propertyName = null
		)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				propertyPropertyChanging?.Invoke(notifyer, new PropertyChangingEventArgs(propertyName));
				field = newValue;
				propertyChanged?.Invoke(notifyer, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
