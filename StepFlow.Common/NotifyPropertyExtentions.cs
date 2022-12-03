using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StepFlow.Common
{
	public static class NotifyPropertyExtentions
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

		public static bool TrySubscrible(INotifyPropertyChanged? obj, PropertyChangedEventHandler handler)
		{
			if (obj is { })
			{
				obj.PropertyChanged += handler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TryUnsubscrible(INotifyPropertyChanged? obj, PropertyChangedEventHandler handler)
		{
			if (obj is { })
			{
				obj.PropertyChanged -= handler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TrySubscrible(INotifyPropertyChanging? obj, PropertyChangingEventHandler handler)
		{
			if (obj is { })
			{
				obj.PropertyChanging += handler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TryUnsubscrible(INotifyPropertyChanging? obj, PropertyChangingEventHandler handler)
		{
			if (obj is { })
			{
				obj.PropertyChanging -= handler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TrySubscrible<T>(T obj, PropertyChangingEventHandler changingHandler, PropertyChangedEventHandler changedHandler)
			where T : INotifyPropertyChanging, INotifyPropertyChanged
		{
			if (obj is { })
			{
				obj.PropertyChanging += changingHandler;
				obj.PropertyChanged += changedHandler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TryUnsubscrible<T>(T obj, PropertyChangingEventHandler changingHandler, PropertyChangedEventHandler changedHandler)
			where T : INotifyPropertyChanging, INotifyPropertyChanged
		{
			if (obj is { })
			{
				obj.PropertyChanging -= changingHandler;
				obj.PropertyChanged -= changedHandler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TrySubscrible(INotifyCollectionChanged? obj, NotifyCollectionChangedEventHandler handler)
		{
			if (obj is { })
			{
				obj.CollectionChanged += handler;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TryUnsubscrible(INotifyCollectionChanged? obj, NotifyCollectionChangedEventHandler handler)
		{
			if (obj is { })
			{
				obj.CollectionChanged -= handler;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
