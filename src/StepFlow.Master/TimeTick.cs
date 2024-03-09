namespace StepFlow.Master
{
	public readonly struct TimeTick
	{
		public const int TICKS_PER_FRAME = 100;
		public const int FRAMES_PER_SECOND = 60;
		public const int TICKS_PER_SECOND = TICKS_PER_FRAME * FRAMES_PER_SECOND;

		public static TimeTick FromFrames(int frames) => new TimeTick(frames * TICKS_PER_FRAME);

		public static TimeTick FromSeconds(int seconds) => new TimeTick(seconds * TICKS_PER_SECOND);

		public TimeTick(int ticks) => Ticks = ticks;

		public int Ticks { get; }

		public float Frames => Ticks / (float)FRAMES_PER_SECOND;

		public float Seconds => Ticks / (float)TICKS_PER_SECOND;

		public static implicit operator int(TimeTick value) => value.Ticks;

		public static implicit operator TimeTick(int value) => new TimeTick(value);
	}
}
