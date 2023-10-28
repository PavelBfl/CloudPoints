using System;

namespace StepFlow.Master.Proxies
{
	[AttributeUsage(AttributeTargets.Interface)]
	public class ComponentProxyAttribute : Attribute
	{
		public ComponentProxyAttribute(Type target, Type proxy, string? name = null)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
			Name = name;
		}

		public string? Name { get; }

		public Type Target { get; }

		public Type Proxy { get; }
	}
}
