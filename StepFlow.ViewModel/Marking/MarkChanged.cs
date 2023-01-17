namespace StepFlow.ViewModel.Marking
{
	public struct MarkChanged<T>
	{
		public MarkChanged(T key, MarkChangedState state)
		{
			Key = key;
			State = state;
		}

		public T Key { get; set; }
		public MarkChangedState State { get; set; }
	}
}
