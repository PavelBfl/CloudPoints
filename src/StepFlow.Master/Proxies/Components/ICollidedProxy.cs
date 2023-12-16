using StepFlow.Core.Components;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<Collided>
	{
		IRefCounter<ShapeBase>? Current { get; set; }
		IShapeBaseProxy<ShapeBase>? CurrentProxy { get; }
		IRefCounter<ShapeBase>? Next { get; set; }
		IShapeBaseProxy<ShapeBase>? NextProxy { get; }

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

			if (Current is { })
			{
				Current.Value.Attached = Target;
			}

			IsMove = false;
		}
	}

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public IRefCounter<ShapeBase>? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

		public IShapeBaseProxy<ShapeBase>? CurrentProxy => (IShapeBaseProxy<ShapeBase>?)Owner.CreateProxy(Target.Current);

		public IRefCounter<ShapeBase>? Next { get => Target.Next; set => SetValue(x => x.Next, value); }

		public IShapeBaseProxy<ShapeBase>? NextProxy => (IShapeBaseProxy<ShapeBase>?)Owner.CreateProxy(Target.Next);

		public bool IsMove { get => Target.IsMove; set => SetValue(x => x.IsMove, value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(x => x.IsRigid, value); }
	}
}
