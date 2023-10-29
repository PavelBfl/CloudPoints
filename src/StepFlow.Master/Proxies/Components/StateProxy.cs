using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	[ComponentProxy(typeof(State), typeof(StateProxy), "State")]
	public interface IStateProxy
	{
		int Team { get; set; }

		SubjectKind Kind { get; set; }
	}

	internal sealed class StateProxy : ComponentProxy<State>, IStateProxy
	{
		public StateProxy(PlayMaster owner, State target) : base(owner, target)
		{
		}

		public int Team { get => Target.Team; set => SetValue(x => x.Team, value); }

		public SubjectKind Kind { get => Target.Kind; set => SetValue(x => x.Kind, value); }
	}
}
