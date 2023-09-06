using System;
using MoonSharp.Interpreter;

namespace StepFlow.Master.Proxies
{
	public interface IComponentProvider
	{
		[MoonSharpHidden]
		public T AddComponentProxy<T>(string name) where T : class;

		[MoonSharpHidden]
		public T GetComponentProxy<T>(string name) where T : class?;

		[MoonSharpHidden]
		public T GetComponentProxyRequired<T>(string name)
			where T : class
			=> GetComponentProxy<T>(name) ?? throw new InvalidOperationException();
	}
}
