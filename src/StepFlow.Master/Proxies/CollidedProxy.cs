using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class CollidedProxy : ComponentProxy<Collided>
	{
		[MoonSharpHidden]
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public Bordered? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

		public Bordered? Next { get => Target.Next; set => SetValue(x => x.Next, value); }

		public bool IsMoving { get => Target.IsMoving; set => SetValue(x => x.IsMoving, value); }

		public float Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		public bool Offset(Point value)
		{
			if (Current is { } current)
			{
				var clone = (Bordered)current.Clone(null);
				clone.Offset(value);
				Next = clone;
				IsMoving = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Move()
		{
			if (IsMoving)
			{
				Current = Next;
				Next = null;
				IsMoving = false;
			}
		}
	}
}
