using System.Collections;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;
using StepFlow.Intersection;

public class Program
{
	private static void Main(string[] args)
	{
		BenchmarkRunner.Run<Test>();
	}
}

public class Test
{
	private static IEnumerable<Rectangle[]> GetRawShapes()
	{
		const int SIZE = 3;

		for (var iX = -10; iX <= 10; iX++)
		{
			for (var iY = -10; iY <= 10; iY++)
			{
				yield return new Rectangle[]
				{
					new(iX, iY, SIZE, SIZE)
				};
			}
		}
	}

	private static Context RelationsContext { get; } = CreateRelationContext();

	private static Context CreateRelationContext()
	{
		var context = new Context();

		foreach (var rawShape in GetRawShapes())
		{
			var shape = Shape.Create(context, rawShape);
			shape.Enable();
		}

		return context;
	}

	private static (Context Context, Shape[] Shapes) SegmentationContext { get; } = CreateSegmentationContext();

	private static (Context Context, Shape[] Shapes) CreateSegmentationContext()
	{
		var context = new StepFlow.Intersection.Segmentation.Context(new Rectangle(-100, -100, 200, 200));
		var shapes = new List<Shape>();

		foreach (var rawShape in GetRawShapes())
		{
			shapes.Add(context.CreateShape(rawShape));
		}

		return (context, shapes.ToArray());
	}

	[Benchmark(Baseline = true)]
	public int Simple()
	{
		var result = 0;

		var control = RelationsContext.Shapes[0];

		for (var i = 1; i < RelationsContext.Shapes.Count; i++)
		{
			if (RelationsContext.Shapes[i].IsIntersection(control))
			{
				result++;
			}
		}

		return result;
	}

	[Benchmark]
	public int Relation()
	{
		var control = RelationsContext.Shapes[0];

		return RelationsContext.GetCollisionInfo(control).GetCollisions().Count();
	}

	[Benchmark]
	public int Segmentation()
	{
		var result = 0;
		foreach (var item in SegmentationContext.Shapes[0].GetCollisions())
		{
			result++;
		}

		return result;
	}
}