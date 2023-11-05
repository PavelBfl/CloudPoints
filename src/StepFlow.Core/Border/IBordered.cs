using System;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core.Border
{
	public interface IBordered
	{
		Rectangle Border { get; }

		void Offset(Point value);

		event EventHandler? BorderChange;

		IEnumerable<IBordered>? Childs { get; }

		IBordered Clone();
	}
}
