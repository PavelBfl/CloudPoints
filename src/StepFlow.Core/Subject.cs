using System.Collections.Generic;
using System.Linq;

namespace StepFlow.Core
{
	public class Subject
	{
		public Subject()
		{
		}

		public string? Name { get; set; }

		public bool Lock { get; set; }

		public virtual IEnumerable<Subject> GetContent() => Enumerable.Empty<Subject>();
	}
}
