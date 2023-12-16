using StepFlow.Core.Components;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<Collided>
	{
		IShapeBaseProxy<ShapeBase>? Current { get; set; }

		IShapeBaseProxy<ShapeBase>? Next { get; set; }

		bool IsMove { get; set; }
		bool IsRigid { get; set; }

		void Move()
		{
			if (IsMove)
			{
				Current = Next;
				Break();
			}
		}

		void Break()
		{
			Next = null;
			IsMove = false;
		}
	}

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public IShapeBaseProxy<ShapeBase>? Current
		{
			get => (IShapeBaseProxy<ShapeBase>?)Owner.CreateProxy(Target.Current);
			set
			{
				Current?.Unregister();
				SetValue(x => x.Current, value?.Target);
				Current?.Register();
			}
		}

		public IShapeBaseProxy<ShapeBase>? Next
		{
			get => (IShapeBaseProxy<ShapeBase>?)Owner.CreateProxy(Target.Next);
			set
			{
				Next?.Unregister();
				SetValue(x => x.Next, value?.Target);
				Next?.Register();
			}
		}

		public bool IsMove { get => Target.IsMove; set => SetValue(x => x.IsMove, value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(x => x.IsRigid, value); }
	}
}
