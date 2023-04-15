using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StepFlow.Common
{
	public static class NotifyPropertyExtensions
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

		public static bool TrySubscribe(INotifyPropertyChanged? obj, PropertyChangedEventHandler handler)
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

		public static bool TryUnsubscribe(INotifyPropertyChanged? obj, PropertyChangedEventHandler handler)
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

		public static bool TrySubscribe(INotifyPropertyChanging? obj, PropertyChangingEventHandler handler)
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

		public static bool TryUnsubscribe(INotifyPropertyChanging? obj, PropertyChangingEventHandler handler)
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

		public static bool TrySubscribe<T>(T obj, PropertyChangingEventHandler changingHandler, PropertyChangedEventHandler changedHandler)
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

		public static bool TryUnsubscribe<T>(T obj, PropertyChangingEventHandler changingHandler, PropertyChangedEventHandler changedHandler)
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

		public static bool TrySubscribe(INotifyCollectionChanged? obj, NotifyCollectionChangedEventHandler handler)
		{
			if (obj is { })
			{
				obj.CollectionChanged += handler;
				handler(obj, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TryUnsubscribe(INotifyCollectionChanged? obj, NotifyCollectionChangedEventHandler handler)
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
