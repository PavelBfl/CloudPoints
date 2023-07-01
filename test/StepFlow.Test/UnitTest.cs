using StepFlow.Master;

namespace StepFlow.Test
{
	public class UnitTest
	{
		[Fact]
		public void Test()
		{
			var master = new PlayMaster();

			for (var x = 0; x < 2; x++)
			{
				for (var y = 0; y < 2; y++)
				{
					master.Playground.Place.Add(master.Playground.CreateNode(x, y));
				}
			}

			var piece = master.Playground.CreatePiece();
			master.Playground.Pieces.Add(piece);
			piece.Current = master.Playground.Place[new(0, 1)];
		}
	}
}