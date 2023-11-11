using System.Drawing;

namespace StepFlow.View.Sketch
{
	public sealed class Layout
	{
		private RectangleF owner;

		public RectangleF Owner
		{
			get => owner;
			set
			{
				if (Owner != value)
				{
					owner = value;
					place = null;
				}
			}
		}

		public Thickness Margin { get; set; }

		public SizeF Size { get; set; }

		private RectangleF? place;

		public RectangleF Place => place ??= Owner.Offset(Margin, Size);
	}
}
