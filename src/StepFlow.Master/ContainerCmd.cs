using System.ComponentModel;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	internal class ContainerCmd<TSource> : WrapperCmd<TSource>, IContainerCmd<TSource>
		where TSource : Container
	{
		public ContainerCmd(PlayMaster owner, TSource source)
			: base(owner, source)
		{
		}

		public void AddComponent(string name) => Owner.TimeAxis.Add(new AddComponent(Source, Owner.CreateComponent(name), name));

		public void RemoveComponent(string name) => Owner.TimeAxis.Add(new Reverse(new AddComponent(Source, Owner.CreateComponent(name), name)));
	}
}
