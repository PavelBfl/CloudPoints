using System;

namespace StepFlow.View.Controls
{
	public class Polygon : PolygonBase
	{
		public Polygon(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		public IReadOnlyVertices? Vertices { get; set; }

		public override IReadOnlyVertices? GetVertices() => Vertices;
	}
}
