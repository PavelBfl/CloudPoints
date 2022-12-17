using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class Polygon : PolygonBase
	{
		public Polygon(Game game) : base(game)
		{
		}

		public List<Vector2>? CustomVertices { get; set; }

		public override IReadOnlyList<Vector2> Vertices => (IReadOnlyList<Vector2>?)CustomVertices ?? Array.Empty<Vector2>();
	}
}
