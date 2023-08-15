using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Master.Commands.Collections;

namespace StepFlow.Master.Proxies
{
	public sealed class SubjectsCollectionProxy : ProxyBase<ICollection<Subject>>, ICollection<Subject>
	{
		[MoonSharpHidden]
		public SubjectsCollectionProxy(PlayMaster owner, ICollection<Subject> target) : base(owner, target)
		{
		}

		public int Count => Target.Count;

		public bool IsReadOnly => false;

		public void Add(Subject item) => Owner.TimeAxis.Add(new AddItemCommand<Subject>(Target, item));

		public void Clear() => Owner.TimeAxis.Add(new ClearCommand<Subject>(Target));

		public bool Contains(Subject item) => Target.Contains(item);

		public void CopyTo(Subject[] array, int arrayIndex) => Target.CopyTo(array, arrayIndex);

		public IEnumerator<Subject> GetEnumerator() => Target.GetEnumerator();

		public bool Remove(Subject item) => Target.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
