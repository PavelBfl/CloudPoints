using System.Drawing;

namespace StepFlow.View.Sketch
{
	public class Layout : ILayoutCanvas
	{
		private ILayoutCanvas? canvas;

		public ILayoutCanvas? Canvas
		{
			get => canvas;
			set
			{
				if (Canvas != value)
				{
					canvas = value;
					place = null;
				}
			}
		}

		public Thickness Margin { get; set; }

		public SizeF Size { get; set; }

		private RectangleF? place;

		public RectangleF Place => place ??= Canvas?.Place.Offset(Margin, Size) ?? RectangleF.Empty;

		public Layout CreateChild(Thickness thickness, SizeF size) => new()
		{
			Canvas = this,
			Margin = thickness,
			Size = size
		};
	}
}
