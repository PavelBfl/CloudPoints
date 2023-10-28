﻿using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	[ComponentProxy(typeof(ProjectileSettings), typeof(ProjectileSettingsProxy), "ProjectileSettingsType")]
	public interface IProjectileSettingsProxy
	{
		Course Course { get; set; }

		int Size { get; set; }

		float Damage { get; set; }

		ICollection<string> Kind { get; }
	}

	internal sealed class ProjectileSettingsProxy : ComponentProxy<ProjectileSettings>, IProjectileSettingsProxy
	{
		public ProjectileSettingsProxy(PlayMaster owner, ProjectileSettings target) : base(owner, target)
		{
		}

		public Course Course { get => Target.Course; set => SetValue(x => x.Course, value); }

		public int Size { get => Target.Size; set => SetValue(x => x.Size, value); }

		public float Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		public ICollection<string> Kind => new CollectionProxy<string, ICollection<string>>(Owner, Target.Kind);
	}
}
