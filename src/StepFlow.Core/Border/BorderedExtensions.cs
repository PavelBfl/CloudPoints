using System;
using System.Drawing;

namespace StepFlow.Core.Border
{
	public static class BorderedExtensions
	{
		public static bool IsCollision(this IBordered main, IBordered other)
		{
			if (main is null)
			{
				throw new ArgumentNullException(nameof(main));
			}

			if (other is null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			if (main.Childs is { } mainChilds)
			{
				if (!main.Border.IntersectsWith(other.Border))
				{
					return false;
				}

				foreach (var child in mainChilds)
				{
					if (child.IsCollision(other))
					{
						return true;
					}
				}

				return false;
			}
			else if (other.Childs is { } otherChilds)
			{
				if (!main.Border.IntersectsWith(other.Border))
				{
					return false;
				}

				foreach (var child in otherChilds)
				{
					if (main.IsCollision(child))
					{
						return true;
					}
				}

				return false;
			}
			else
			{
				return main.Border.IntersectsWith(other.Border);
			}
		}

		public static bool Contains(this IBordered main, Point point)
		{
			if (main is null)
			{
				throw new ArgumentNullException(nameof(main));
			}

			if (main.Childs is { } childs)
			{
				if (!main.Border.Contains(point))
				{
					return false;
				}

				foreach (var child in childs)
				{
					if (child.Contains(point))
					{
						return true;
					}
				}

				return false;
			}
			else
			{
				return main.Border.Contains(point);
			}
		}
	}
}
