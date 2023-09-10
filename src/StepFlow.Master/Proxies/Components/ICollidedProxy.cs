﻿using System.Drawing;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IComponentProxy
	{
		Bordered? Current { get; set; }

		Bordered? Next { get; set; }

		bool IsMoving { get; set; }

		void Break();

		void Move();

		bool Offset(Point value);
	}
}