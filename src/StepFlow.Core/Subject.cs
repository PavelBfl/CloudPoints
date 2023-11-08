using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.Core
{
	public class Subject
	{
		public Subject(Context owner)
		{
			Context = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Context Context { get; }

		public string? Name { get; set; }

		public bool Lock { get; set; }

		public virtual IEnumerable<Subject> GetContent() => Enumerable.Empty<Subject>();
	}
}
