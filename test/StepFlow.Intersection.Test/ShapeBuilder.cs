using System.Collections.Immutable;
using System.Drawing;

namespace StepFlow.Intersection.Test;

public readonly record struct ShapeBuilder(ImmutableArray<Rectangle> Rectangles)
{
	public ShapeBase Create(Context context) => ShapeBase.Create(context, Rectangles);

	public bool Intersect(IEnumerable<Rectangle> other)
	{
		foreach (var rectangle in Rectangles)
		{
			foreach (var otherRectangle in other)
			{
				if (rectangle.IntersectsWith(otherRectangle))
				{
					return true;
				}
			}
		}

		return false;
	}
}
