using System.Drawing;
using StepFlow.Common;

namespace StepFlow.Layout
{
	public class RectPlot : BaseNotifyer
	{
		private RectangleF ownerBounds;

		public RectangleF OwnerBounds
		{
			get => ownerBounds;
			set
			{
				if (OwnerBounds != value)
				{
					ownerBounds = value;
					CleanBounds();
					OnPropertyChanged();
				}
			}
		}

		private RectangleF? bounds;

		public RectangleF Bounds => bounds ??= CreateBound();

		private Margin margin;

		public Margin Margin
		{
			get => margin;
			set
			{
				if (Margin != value)
				{
					margin = value;
					CleanBounds();
					OnPropertyChanged();
				}
			}
		}

		private SizeF size;

		public SizeF Size
		{
			get => size;
			set
			{
				if (Size != value)
				{
					size = value;
					CleanBounds();
					OnPropertyChanged();
				}
			}
		}

		private void CleanBounds()
		{
			bounds = null;
			OnPropertyChanged(nameof(Bounds));
			OnChangeBounds();
		}

		protected virtual void OnChangeBounds()
		{
		}

		private RectangleF CreateBound()
		{
			var horizontal = GetAlign(OwnerBounds.Left, OwnerBounds.Right, Margin.Left, Margin.Right, Size.Width);
			var vertical = GetAlign(OwnerBounds.Top, OwnerBounds.Bottom, Margin.Top, Margin.Bottom, Size.Height);

			return new RectangleF(
				x: horizontal.Min,
				y: vertical.Min,
				width: horizontal.Max - horizontal.Min,
				height: vertical.Max - vertical.Min
			);
		}

		private static (float Min, float Max) GetAlign(float globalMin, float globalMax, float? min, float? max, float length)
		{
			return (min, max) switch
			{
				(float minValue, float maxValue) => (globalMin + minValue, globalMax - maxValue),
				(float minValue, null) => (globalMin + minValue, globalMin + minValue + length),
				(null, float maxValue) => (globalMax - length - maxValue, globalMax - maxValue),
				(null, null) => (globalMin + (globalMax - globalMin - length) / 2, globalMax - (globalMax - globalMin - length) / 2)
			};
		}
	}
}
