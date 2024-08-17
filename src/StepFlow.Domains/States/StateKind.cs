﻿namespace StepFlow.Domains.States
{
	public enum StateKind
	{
		Remove,
		Poison,
		Arc,
		Push,
		Dash,
		CreatingProjectile,
		Gravity,

		EnemySerpentineForwardState,
		EnemySerpentineForwardStateAttack,
		EnemySerpentineForwardToBackward,
		EnemySerpentineBackwardState,
		EnemySerpentineBackwardStateAttack,
		EnemySerpentineBackwardToForward,
	}
}
