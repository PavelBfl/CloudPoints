namespace StepFlow.Core.Commands
{
	internal class Scheduler<T> : IScheduler<T>, ITargetingContainer<T>
	{
		public Scheduler(T target)
		{
			Target = target;
			Queue = new Queue<T>();
			Builders = new BuildersCollection<T>(this);
		}

		public T Target { get; }

		public IBuildersCollection<T> Builders { get; }

		public Queue<T> Queue { get; }

		IQueue<T> IScheduler<T>.Queue => Queue;
	}
}
