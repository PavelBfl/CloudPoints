using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public interface ICell
	{
		Point Current { get; set; }

		Point Next { get; set; }

		bool IsStatic => Current == Next;

		bool IsMoved => !IsStatic;

		void TakeStep() => Current = Next;
	}

	public sealed class Cell
	{
		public Point Current { get; set; }

		public Point Next { get; set; }

		public bool IsStatic => Current == Next;

		public bool IsMoved => !IsStatic;

		public void TakeStep() => Current = Next;
	}

	public class Playground : Container
	{
		public ICollection<ICell> Cells { get; } = new HashSet<ICell>();

		public IEnumerable<ICollection<ICell>> DeclareCollision()
		{
			var counter = new Dictionary<Point, HashSet<ICell>>();

			foreach (var cell in Cells)
			{
				if (!counter.TryGetValue(cell.Next, out var candidates))
				{
					candidates = new HashSet<ICell>();
					counter.Add(cell.Next, candidates);
				}

				candidates.Add(cell);
			}

			return counter.Values.Where(x => x.Count > 1);
		}
	}
}
