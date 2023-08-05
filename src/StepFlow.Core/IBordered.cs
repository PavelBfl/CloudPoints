using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core
{
	public interface IBordered
	{
		Rectangle Border { get; }

		IEnumerable<IBordered>? Childs { get; }
	}
}
