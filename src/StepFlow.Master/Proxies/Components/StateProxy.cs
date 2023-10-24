using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IStateProxy
	{
		int Team { get; set; }
	}

	internal sealed class StateProxy : ComponentProxy<State>, IStateProxy
	{
		public StateProxy(PlayMaster owner, State target) : base(owner, target)
		{
		}

		public int Team { get => Target.Team; set => SetValue(x => x.Team, value); }
	}
}
