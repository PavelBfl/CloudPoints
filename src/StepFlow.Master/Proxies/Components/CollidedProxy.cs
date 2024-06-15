using System.Drawing;
using System.Linq;
using System.Numerics;
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

		Vector2 Position { get; }

		void SetPosition(Vector2 value, bool asMove);

		bool IsMove { get; set; }

		bool IsRigid { get; set; }

		void Move()
		{
			if (IsMove)
			{
				CurrentProxy.Reset(Next);
				NextProxy.Clear();
				IsMove = false;
			}
		}

		void Break()
		{
			NextProxy.Clear();
			IsMove = false;

			SetPosition(
				new Vector2(
					Current.Bounds.Location.X,
					Current.Bounds.Location.Y
				),
				false
			);
		}

		void Clear()
		{
			CurrentProxy.Clear();
			NextProxy.Clear();
			SetPosition(Vector2.Zero, false);
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

		public Vector2 Position => Target.Position;

		public void SetPosition(Vector2 value, bool asMove)
		{
			if (SetValue(value, nameof(Collided.Position)) && asMove)
			{
				// Скорректировать конвертацию в int
				var newLocation = new Point(
					(int)Position.X,
					(int)Position.Y
				);

				if (Current.Bounds.Location != newLocation)
				{
					var offset = new Point(
						newLocation.X - Current.Bounds.Location.X,
						newLocation.Y - Current.Bounds.Location.Y
					);

					var next = Current.AsEnumerable().Offset(offset);
					ICollidedProxy collidedProxy = this;
					collidedProxy.NextProxy.Reset(next);
					collidedProxy.IsMove = true;
				}
			}
		}

		public bool IsMove { get => Target.IsMove; set => SetValue(value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(value); }
	}
}
