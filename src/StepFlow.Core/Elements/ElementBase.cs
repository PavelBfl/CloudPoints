using System;
using StepFlow.Core.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public abstract class ElementBase : Subject
	{
		protected void SetComponent<T>(ref T current, T value)
			where T : ComponentBase?
		{
			if (current is { } && current.Element != this)
			{
				throw new InvalidOperationException();
			}

			if (value is { } && value.Element != null)
			{
				throw new InvalidOperationException();
			}

			if (current is { })
			{
				current.Element = null;
			}

			if (value is { })
			{
				value.Element = this;
			}

			current = value;
		}

		public ElementBase(IContext context)
			: base(context)
		{
		}

		public ElementBase(IContext context, ElementBaseDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);
		}

		public void CopyTo(ElementBaseDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);
		}
	}
}
