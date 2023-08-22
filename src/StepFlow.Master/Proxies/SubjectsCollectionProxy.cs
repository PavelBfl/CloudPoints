using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Master.Commands.Collections;

namespace StepFlow.Master.Proxies
{
	public sealed class SubjectsCollectionProxy : ProxyBase<IList<Subject>>, IList<Subject>
	{
		[MoonSharpHidden]
		public SubjectsCollectionProxy(PlayMaster owner, IList<Subject> target) : base(owner, target)
		{
		}

		// TODO Добавить команду
		public Subject this[int index] { get => Target[index]; set => Target[index] = value; }

		public int Count => Target.Count;

		public bool IsReadOnly => false;

		public void Add(Subject item) => Owner.TimeAxis.Add(new AddItemCommand<Subject>(Target, item));

		public void Clear() => Owner.TimeAxis.Add(new ClearCommand<Subject>(Target));

		public bool Contains(Subject item) => Target.Contains(item);

		public void CopyTo(Subject[] array, int arrayIndex) => Target.CopyTo(array, arrayIndex);

		public IEnumerator<Subject> GetEnumerator() => Target.GetEnumerator();

		public int IndexOf(Subject item) => Target.IndexOf(item);

		// TODO Добавить команду
		public void Insert(int index, Subject item) => Target.Insert(index, item);

		public bool Remove(Subject item) => Target.Remove(item);

		// TODO Добавить команду
		public void RemoveAt(int index) => Target.RemoveAt(index);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
