using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Components;
using StepFlow.Domains.Components;
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

		Vector2 Offset { get; set; }

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
		}

		void Clear()
		{
			CurrentProxy.Clear();
			NextProxy.Clear();
			Offset = Vector2.Zero;
		}

		void SetOffset()
		{
			var localOffset = new Point(
				(int)Offset.X,
				(int)Offset.Y
			);

			if (Math.Abs(localOffset.X) > 0 || Math.Abs(localOffset.Y) > 0)
			{
				var next = Current.AsEnumerable().Offset(localOffset);
				NextProxy.Reset(next);
				IsMove = true;

				Offset -= new Vector2(localOffset.X, localOffset.Y);
			}
		}

		void CopyFrom(CollidedDto original)
		{
			NullValidateExtensions.ThrowIfArgumentNull(original, nameof(original));

			CurrentProxy.Reset(original.Current);
			NextProxy.Reset(original.Next);
			Offset = original.Offset;
			IsMove = original.IsMove;
			IsRigid = original.IsRigid;
		}
	}

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public ShapeContainer Current { get => Target.Current; }

		public ShapeContainer Next { get => Target.Next; }

		public Vector2 Offset { get => Target.Offset; set => SetValue(value); }

		public bool IsMove { get => Target.IsMove; set => SetValue(value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(value); }
	}
}
