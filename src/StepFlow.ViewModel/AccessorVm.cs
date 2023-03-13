﻿using System;
using System.Diagnostics.CodeAnalysis;
using StepFlow.ViewModel.Services;

namespace StepFlow.ViewModel
{
	internal struct AccessorVm<T>
	{
		public bool IsReceived { get; private set; }

		[AllowNull]
		[MaybeNull]
		private T Value { get; set; }

		[return: MaybeNull]
		public T GetValue(IWrapperProvider wrapperProvider, object? model)
		{
			if (wrapperProvider is null)
			{
				throw new ArgumentNullException(nameof(wrapperProvider));
			}

			if (model is null)
			{
				Value = default;
				IsReceived = true;
			}
			else if (!IsReceived)
			{
				Value = (T)wrapperProvider.GetViewModelOrDefault(model);
				IsReceived = true;
			}

			return Value;
		}
	}
}
