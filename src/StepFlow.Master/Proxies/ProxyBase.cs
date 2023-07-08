using System;
using MoonSharp.Interpreter;

namespace StepFlow.Master.Proxies
{
	public class ProxyBase<TTarget>
		where TTarget : class
	{
		[MoonSharpHidden]
		public ProxyBase(PlayMaster owner, TTarget target)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Target = target ?? throw new ArgumentNullException(nameof(target));
		}

		public PlayMaster Owner { get; }

		public TTarget Target { get; }
	}
}
