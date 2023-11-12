namespace StepFlow.Core.Components
{
	public interface IScale
	{
		int Value { get; set; }

		int Max { get; set; }

		bool Freeze { get; set; }
	}

	public sealed class Scale : Subject, IScale
	{
		public Scale(Context owner) : base(owner)
		{
		}

		public int Value { get; set; }

		public int Max { get; set; }

		public bool Freeze { get; set; }
	}
}
