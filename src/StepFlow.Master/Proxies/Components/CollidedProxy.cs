using System;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class CollidedProxy : ComponentProxy<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public IBorderedProxy? Current { get => Owner.CreateProxy(Target.Current); set => SetValue(x => x.Current, value?.Target); }

		public IBorderedProxy? Next { get => Owner.CreateProxy(Target.Next); set => SetValue(x => x.Next, value?.Target); }

		public bool IsMoving { get => Target.IsMoving; set => SetValue(x => x.IsMoving, value); }

		public bool Offset(Point value)
		{
			if (Current is { } current)
			{
				var clone = (Bordered)current.Target.Clone(null);
				clone.Offset(value);
				Next = Owner.CreateProxy(clone);
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
				Break();
			}
		}

		public void Break()
		{
			if (IsMoving)
			{
				Next = null;
				IsMoving = false;
			}
		}
	}
}
