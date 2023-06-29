using System;
using System.ComponentModel;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	internal class ContainerCmd<TSource> : IContainerCmd<TSource>
		where TSource : Container
	{
		public ContainerCmd(PlayMaster owner, TSource source)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public PlayMaster Owner { get; }

		public TSource Source { get; }

		public void AddComponent(string name) => Owner.TimeAxis.Add(new AddComponent(Source, Owner.CreateComponent(name), name));

		public void RemoveComponent(string name) => Owner.TimeAxis.Add(new Reverse(new AddComponent(Source, Owner.CreateComponent(name), name)));
	}
}
