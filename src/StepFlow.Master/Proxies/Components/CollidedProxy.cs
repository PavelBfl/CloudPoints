using StepFlow.Core.Components;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<Collided>
	{
		ShapeBase? Current { get; set; }

		ShapeBase? Next { get; set; }

		bool IsMove { get; set; }

		bool IsRigid { get; set; }

		void Move()
		{
			if (IsMove)
			{
				Current = Next?.Clone();
				Break();
			}
		}

		void Break()
		{
			Next = null;
			IsMove = false;
		}

		void Clear()
		{
			Current = null;
			Next = null;
		}
	}

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public ShapeBase? Current { get => Target.Current; set => SetValue(value); }

		public ShapeBase? Next { get => Target.Next; set => SetValue(value); }

		public bool IsMove { get => Target.IsMove; set => SetValue(value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(value); }
	}
}
