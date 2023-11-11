using MoonSharp.Interpreter;

namespace StepFlow.Master.Proxies
{
	public interface IReadOnlyProxyBase<out TTarget>
	{
		[MoonSharpHidden]
		PlayMaster Owner { get; }

		[MoonSharpHidden]
		TTarget Target { get; }
	}
}
