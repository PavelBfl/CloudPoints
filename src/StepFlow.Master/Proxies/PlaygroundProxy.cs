﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class PlaygroundProxy : ContainerProxy<Playground>
	{
		[MoonSharpHidden]
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		[MoonSharpHidden]
		public SubjectProxy<Subject> CreateSubjectProxy(bool addContainer = true)
		{
			var result = (SubjectProxy<Subject>)Owner.CreateProxy(CreateSubject());
			if (addContainer)
			{
				Subjects.Add(result.Target);
			}
			return result;
		}

		public SubjectsCollectionProxy Subjects => new SubjectsCollectionProxy(Owner, Target.Subjects);

		public Subject CreateSubject() => new Subject(Target);

		public IEnumerable<(Subject, Subject)> GetCollision() => Target.GetCollision();

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public Bordered CreateBordered() => new Bordered();
	}
}
