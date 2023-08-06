using System.Drawing;

namespace StepFlow.Core
{
	public interface IBorderedNode : IBordered
	{
		void Offset(Point point);

		IBorderedNode Clone(Bordered? owner);
	}
}
