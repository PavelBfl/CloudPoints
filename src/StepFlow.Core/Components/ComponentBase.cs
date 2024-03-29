﻿using StepFlow.Core.Elements;

namespace StepFlow.Core.Components
{
	public class ComponentBase : Subject
	{
		public ElementBase? Element { get; set; }

		public ElementBase GetElementRequired() => PropertyRequired(Element, nameof(Element));
	}
}
