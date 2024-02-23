using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public class Scheduler : ElementBase
	{
		public Turn? Current { get; set; }
	}

	public sealed class SchedulerPath : Scheduler
	{
		public int CurrentIndex { get; set; }

		public IList<Course> Path { get; } = new List<Course>();

		public bool IsLast { get; set; }

		public Turn? Last { get; set; }
	}

	public sealed class SchedulerVector : Scheduler
	{
		public Collided? Collided { get; set; }

		public ICollection<Vector2> Vectors { get; } = new List<Vector2>();

		public Vector2 CorrectVector { get; set; }

		public int IndexCourse { get; set; }
	}

	public sealed class SchedulerLimit : Scheduler
	{
		public Scheduler? Source { get; set; }

		public Scale? Range { get; set; }
	}

	public sealed class SchedulerCollection : Scheduler
	{
		public int Index { get; set; }

		public IList<Turn> Turns { get; } = new List<Turn>();
	}

	public sealed class SchedulerUnion : Scheduler
	{
		public int Index { get; set; }

		public IList<Scheduler> Schedulers { get; } = new List<Scheduler>();
	}

	public sealed class SchedulerRunner : Subject
	{
		public int Begin { get; set; }

		public Turn? Current { get; set; }

		public Scheduler? Scheduler { get; set; }
	}
}
