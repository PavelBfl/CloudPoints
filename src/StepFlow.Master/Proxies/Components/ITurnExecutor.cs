using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface ITurnExecutor : IProxyBase<Subject>
	{
		void Execute();
	}
}
