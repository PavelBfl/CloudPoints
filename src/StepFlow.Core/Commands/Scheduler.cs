namespace StepFlow.Core.Commands
{
	internal class Scheduler<T> : IScheduler<T>
	{
		public Scheduler(T target)
		{
			Container = new TargetingContainer<T>(target);
			Builders = new BuildersCollection<T>(Container);
			Queue = new Queue<T>(Container.Commands);
		}

		private TargetingContainer<T> Container { get; }

		public IBuildersCollection<T> Builders { get; }

		public IQueue<T> Queue { get; }
	}
}
