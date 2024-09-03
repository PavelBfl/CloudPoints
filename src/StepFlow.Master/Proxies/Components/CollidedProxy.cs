﻿using System;
using System.Drawing;
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
		Shape? Current { get; set; }

		IShapeProxy? CurrentProxy => (IShapeProxy?)Owner.CreateProxy(Current);

		Shape? Next { get; set; }

		IShapeProxy? NextProxy => (IShapeProxy?)Owner.CreateProxy(Next);

		bool IsMove { get; set; }

		bool IsRigid { get; set; }

		Vector2 Offset { get; set; }

		void Move()
		{
			if (IsMove)
			{
				var next = Next;
				Next = null;
				Current = next;
				IsMove = false;
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
			Offset = Vector2.Zero;
		}

		void SetOffset();

		void CopyFrom(CollidedDto original);
	}

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public Shape? Current { get => Target.Current; set => SetValue(value); }

		public Shape? Next { get => Target.Next; set => SetValue(value); }

		public Vector2 Offset { get => Target.Offset; set => SetValue(value); }

		public bool IsMove { get => Target.IsMove; set => SetValue(value); }

		public bool IsRigid { get => Target.IsRigid; set => SetValue(value); }

		public void SetOffset()
		{
			var localOffset = new Point(
				(int)Offset.X,
				(int)Offset.Y
			);

			var absOffset = new Point(Math.Abs(localOffset.X), Math.Abs(localOffset.Y));
			if (absOffset.X > 0 || absOffset.Y > 0)
			{
				Next = Current?.Clone(localOffset);
				IsMove = true;

				Offset -= new Vector2(localOffset.X, localOffset.Y);
			}
		}

		public void CopyFrom(CollidedDto original)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			Current = Owner.CreateShape(original.Current);
			Next = Owner.CreateShape(original.Next);

			Offset = original.Offset;
			IsMove = original.IsMove;
			IsRigid = original.IsRigid;
		}
	}
}
