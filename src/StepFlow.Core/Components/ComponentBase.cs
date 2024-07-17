using StepFlow.Core.Elements;
using StepFlow.Domains.Components;

namespace StepFlow.Core.Components
{
	public abstract class ComponentBase : Subject
	{
		public ComponentBase(IContext context)
			: base(context)
		{
		}

		public ComponentBase(IContext context, ComponentBaseDto original)
			: base(context)
		{
			CopyExtensions.ThrowIfOriginalNull(original);
		}

		public ElementBase? Element { get; set; }

		public ElementBase GetElementRequired() => PropertyRequired(Element, nameof(Element));

		public void CopyTo(ComponentBaseDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);
		}
	}
}
