using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public abstract class ElementBase : Subject
	{
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
