﻿using System;
using System.Drawing;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<Material.Collided>
	{
		Shape? Current { get; set; }

		Shape? Next { get; set; }

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

		bool SetNextPosition(Vector2 position)
		{
			if (Current is { })
			{
				var currentPosition = Current.Bounds.Location;
				var newPosition = new Point(
					(int)position.X,
					(int)position.Y
				);

				if (currentPosition != newPosition)
				{
					Next = Shape.Create(Current.Offset(new Point(newPosition.X - currentPosition.X, newPosition.Y - currentPosition.Y)));
					IsMove = true;
				}
				else
				{
					Next = null;
					IsMove = false;
				}

				Offset = position - new Vector2(newPosition.X, newPosition.Y);

				return true;
			}
			else
			{
				return false;
			}
		}

		void SetOffset();

		void CopyFrom(MaterialDto original);
	}

	internal sealed class CollidedProxy : ProxyBase<Material.Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Material.Collided target) : base(owner, target)
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
				Next = Current?.Offset(localOffset) is { } rectangles ? Shape.Create(rectangles) : null;
				IsMove = true;

				Offset -= new Vector2(localOffset.X, localOffset.Y);
			}
		}

		public void CopyFrom(MaterialDto original)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			Current = Owner.CreateShape(original.BodyCurrent);
			Next = Owner.CreateShape(original.BodyNext);

			Offset = original.BodyOffset;
			IsMove = original.BodyIsMove;
			IsRigid = original.IsRigid;
		}
	}
}
