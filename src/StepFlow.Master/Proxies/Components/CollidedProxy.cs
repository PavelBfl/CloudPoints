using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	[ComponentProxy(typeof(Collided), typeof(CollidedProxy), "CollidedType")]
	public interface ICollidedProxy : IComponentProxy
	{
		IBorderedProxy? Current { get; set; }

		IBorderedProxy? Next { get; set; }

		bool IsMoving { get; set; }

		ICollection<IHandlerProxy> Collision { get; }
		bool IsRigid { get; set; }

		void Break();

		void Move();

		bool Offset(Point value);
	}

	internal sealed class CollidedProxy : ComponentProxy<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public IBorderedProxy? Current { get => (IBorderedProxy?)Owner.CreateProxy(Target.Current); set => SetValue(x => x.Current, value?.Target); }

		public IBorderedProxy? Next { get => (IBorderedProxy?)Owner.CreateProxy(Target.Next); set => SetValue(x => x.Next, value?.Target); }

		public bool IsMoving { get => Target.IsMoving; set => SetValue(x => x.IsMoving, value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(x => x.IsRigid, value); }

		public ICollection<IHandlerProxy> Collision => CreateEvenProxy(Target.Collision);

		public bool Offset(Point value)
		{
			if (Current is { } current)
			{
				var clone = (Bordered)current.Target.Clone(null);
				clone.Offset(value);
				Next = (IBorderedProxy)Owner.CreateProxy(clone);
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
