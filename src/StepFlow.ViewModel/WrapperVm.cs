using System;
using System.Diagnostics.CodeAnalysis;
using StepFlow.Common;
using StepFlow.ViewModel.Exceptions;
using System.Runtime.CompilerServices;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm
	{
		public WrapperVm(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		[AllowNull]
		[MaybeNull]
		internal virtual T Source { get; set; }

		internal T SourceRequired => Source.PropertyRequired(nameof(Source));

		protected T UsePropertySourceRequired([CallerMemberName] string? propertyName = null)
			=> Source is { } ? Source : throw InvalidAccessToMember.CreateInvalidAccessToProperty(propertyName);

		protected T UseMethodSourceRequired([CallerMemberName] string? methodName = null)
			=> Source is { } ? Source : throw InvalidAccessToMember.CreateInvalidInvokeMethod(methodName);
	}
}
