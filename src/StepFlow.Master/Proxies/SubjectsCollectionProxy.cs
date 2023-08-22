﻿using System.Collections.Generic;
using MoonSharp.Interpreter;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public sealed class SubjectsCollectionProxy : ListProxy<Subject, IList<Subject>>
	{
		[MoonSharpHidden]
		public SubjectsCollectionProxy(PlayMaster owner, IList<Subject> target) : base(owner, target)
		{
		}
	}
}
