using StepFlow.Core.Elements;
using StepFlow.Domains.Components;

namespace StepFlow.Core.Components
{
	public class ComponentBase : Subject
	{
		public ComponentBase()
		{
		}

		public ComponentBase(ComponentBaseDto original)
		{
			ThrowIfOriginalNull(original);
		}

		public ElementBase? Element { get; set; }

		public ElementBase GetElementRequired() => PropertyRequired(Element, nameof(Element));
	}
}
