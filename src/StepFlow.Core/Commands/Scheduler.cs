namespace StepFlow.Core.Commands
{
	internal class Scheduler<T> : IScheduler<T>
	{
		public Scheduler(T target)
		{
			Target = target;
			Queue = new Queue<T>(target);
			Builders = new BuildersCollection<T>(Queue);
		}

		public T Target { get; }

		public IBuildersCollection<T> Builders { get; }

		public Queue<T> Queue { get; }

		IQueue<T> IScheduler<T>.Queue => Queue;
	}
}
