﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentController
	{
		IComponentProxy AddComponent(string name);

		bool RemoveComponent(string name);

		IComponentProxy? GetComponent(string? name);

		IComponentProxy GetComponentRequired(string name) => GetComponent(name) ?? throw new InvalidOperationException();
	}
}
