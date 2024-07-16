namespace StepFlow.Domains.States
{
	public enum StateKind
	{
		Remove,
		Poison,
		Arc,
		Push,
		Dash,
		CreatingProjectile,

		EnemySerpentineForwardState,
		EnemySerpentineForwardStateAttack,
		EnemySerpentineForwardToBackward,
		EnemySerpentineBackwardState,
		EnemySerpentineBackwardStateAttack,
		EnemySerpentineBackwardToForward,
	}
}
