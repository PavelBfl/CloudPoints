using StepFlow.Core.Components;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<Collided>
	{
		ShapeContainer Current { get; }

		IShapeContainerProxy CurrentProxy => (IShapeContainerProxy)Owner.CreateProxy(Current);

		ShapeContainer Next { get; }

		IShapeContainerProxy NextProxy => (IShapeContainerProxy)Owner.CreateProxy(Next);

		bool IsMove { get; set; }

		bool IsRigid { get; set; }

		void Move()
		{
			if (IsMove)
			{
				CurrentProxy.Reset(Next);
				Break();
			}
		}

		void Break()
		{
			NextProxy.Clear();
			IsMove = false;
		}

		void Clear()
		{
			CurrentProxy.Clear();
			NextProxy.Clear();
		}

		void Unregister()
		{
			CurrentProxy.Unregister();
			NextProxy.Unregister();
		}

		void Register()
		{
			CurrentProxy.Register();
			NextProxy.Register();
		}
	}

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public ShapeContainer Current { get => Target.Current; }

		public ShapeContainer Next { get => Target.Next; }

		public bool IsMove { get => Target.IsMove; set => SetValue(value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(value); }
	}
}
