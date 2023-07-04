using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace StepFlow.Master
{
	public interface IPlaceCmd : IReadOnlyDictionary<Point, INodeCmd>
	{
		INodeCmd this[int x, int y] { get; }

		void Add(INodeCmd node);
		bool Remove(Point position);
	}
}
