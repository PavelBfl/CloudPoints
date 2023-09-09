using MoonSharp.Interpreter;

namespace StepFlow.Master.Proxies
{
	public interface IProxyBase<out TTarget>
		where TTarget : class
	{
		[MoonSharpHidden]
		PlayMaster Owner { get; }

		[MoonSharpHidden]
		TTarget Target { get; }
	}
}
