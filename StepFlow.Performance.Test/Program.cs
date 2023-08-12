using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using StepFlow.Core;

public class Program
{
	private static void Main(string[] args)
	{
		BenchmarkRunner.Run<Test>();
	}

	private static IEnumerable<Bordered> CreateBorders(int xCount, int yCount, int offset, int diameter)
	{
		for (var iX = 0; iX < xCount; iX++)
		{
			for (var iY = 0; iY < yCount; iY++)
			{
				var border = CreateBordered(diameter);
				border.Offset(new(iX * offset, iY * offset));
				yield return border;
			}
		}
	}

	private static Bordered CreateBordered(int diameter)
	{
		var bordered = new StepFlow.Core.Bordered();
		foreach (var point in Create(diameter))
		{
			bordered.AddCell(new(point, new System.Drawing.Size(1, 1)));
		}
		return bordered;
	}

	private static IEnumerable<System.Drawing.Point> Create(int diameter)
	{
		if (diameter == 0)
		{
			yield return System.Drawing.Point.Empty;
		}
		else
		{
			var begin = diameter / 2;
			var radius = diameter / 2f;
			var radiusD = radius * radius;
			for (var iX = -begin; iX < diameter; iX++)
			{
				for (var iY = -begin; iY < diameter; iY++)
				{
					if ((iY * iY) + (iX * iX) <= radiusD)
					{
						yield return new(iX, iY);
					}
				}
			}
		}
	}

	public class Test
	{
		private static Bordered[] Bordereds25 { get; } = CreateBorders(5, 5, 5, 10).ToArray();
		private static Bordered[] Bordereds100 { get; } = CreateBorders(10, 10, 5, 10).ToArray();

		private int CollisionCount(Bordered[] bordereds)
		{
			var result = 0;
			foreach (var first in bordereds)
			{
				foreach (var second in bordereds)
				{
					if (first.IsCollision(second))
					{
						result++;
					}
				}
			}

			return result;
		}

		[Benchmark]
		public int CollisionCount25() => CollisionCount(Bordereds25);

		[Benchmark]
		public int CollisionCount100() => CollisionCount(Bordereds100);
	}
}