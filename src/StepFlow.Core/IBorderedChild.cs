using System.Drawing;

namespace StepFlow.Core
{
	public interface IBorderedChild : IBordered
	{
		void Offset(Point point);
	}
}
