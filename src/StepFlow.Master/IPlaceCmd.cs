using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Master
{
	public interface IPlaceCmd : IReadOnlyDictionary<Point, INodeCmd>
	{
		void Add(INodeCmd node);
		bool Remove(Point position);
	}
}
