using System;
using System.ComponentModel;
using StepFlow.TimeLine;

namespace StepFlow.Master.Commands
{
	public sealed class AddComponent : ICommand
	{
		public AddComponent(Container container, IComponent component, string? componentName)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
			Component = component ?? throw new ArgumentNullException(nameof(component));
			ComponentName = componentName;
		}

		public Container Container { get; }

		public IComponent Component { get; }

		public string? ComponentName { get; }

		public void Execute() => Container.Add(Component, ComponentName);

		public void Revert() => Container.Remove(Component);
	}
}
