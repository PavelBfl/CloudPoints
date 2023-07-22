using System;
using System.Drawing;

namespace StepFlow.View.Sketch
{
	public static class RectangleExtensions
	{
		public static RectangleF Offset(this RectangleF rectangle, Thickness offset, SizeF size)
		{
			(float left, float right) = LinearOffset(rectangle.Left, rectangle.Right, offset.Left, offset.Right, size.Width);
			(float top, float bottom) = LinearOffset(rectangle.Top, rectangle.Bottom, offset.Top, offset.Bottom, size.Height);

			return RectangleF.FromLTRB(left, top, right, bottom);
		}

		private static (float min, float max) LinearOffset(float min, float max, Unit byMin, Unit byMax, float size)
		{
			float? resultMin = byMin.Kind switch
			{
				UnitKind.None => null,
				UnitKind.Pixels => min + byMin.Value,
				UnitKind.Ptc => Lerp(min, max, byMin.Value),
				_ => throw new InvalidOperationException(),
			};

			float? resultMax = byMax.Kind switch
			{
				UnitKind.None => null,
				UnitKind.Pixels => max - byMax.Value,
				UnitKind.Ptc => Lerp(min, max, byMax.Value),
				_ => throw new InvalidOperationException(),
			};

			if (resultMin is null)
			{
				if (resultMax is null)
				{
					var centerMin = min + ((max - min) - size) / 2;
					return (centerMin, centerMin + size);
				}
				else
				{
					return (resultMax.Value - size, resultMax.Value);
				}
			}
			else
			{
				if (resultMax is null)
				{
					return (resultMin.Value, resultMin.Value + size);
				}
				else
				{
					return (resultMin.Value, resultMax.Value);
				}
			}
		}

		private static float Lerp(float min, float max, float value) => (max - min) * value + min;
	}
}
