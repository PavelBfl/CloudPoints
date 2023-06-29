﻿namespace StepFlow.Master
{
	public interface IContainerCmd<out T> : IWrapperCmd<T>
	{
		void AddComponent(string name);

		void RemoveComponent(string name);
	}
}
