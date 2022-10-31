using CloudPoints;
using TimeLine;

namespace Core
{
	public class World
	{
		public Graph<HexNode, int> Grid { get; } = new Graph<HexNode, int>();

		public Axis TimeAxis { get; } = new Axis();
	}
}
