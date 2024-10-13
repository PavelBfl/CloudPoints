using StepFlow.Common.Exceptions;
using System.Drawing;

namespace StepFlow.Common.Drawing.Layout
{
	public static class RectangleExtensions
	{
		public static RectangleF Offset(this RectangleF rectangle, float thickness) => RectangleF.FromLTRB(
			rectangle.Left + thickness,
			rectangle.Top + thickness,
			rectangle.Right - thickness,
			rectangle.Bottom - thickness
		);

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
				_ => throw EnumNotSupportedException.Create(byMin.Kind),
			};

			float? resultMax = byMax.Kind switch
			{
				UnitKind.None => null,
				UnitKind.Pixels => max - byMax.Value,
				UnitKind.Ptc => Lerp(min, max, byMax.Value),
				_ => throw EnumNotSupportedException.Create(byMax.Kind),
			};

			if (resultMin is null)
			{
				if (resultMax is null)
				{
					var centerMin = min + (max - min - size) / 2;
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
