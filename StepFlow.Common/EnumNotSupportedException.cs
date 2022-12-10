using System;

namespace StepFlow.Common
{
	public class EnumNotSupportedException : NotSupportedException
	{
		private const string DEFAULT_MESSAGE = "Значение перечисления не потдерживается.";

		public static EnumNotSupportedException Create<T>(T value)
			where T : struct, Enum
			=> new EnumNotSupportedException(typeof(T), Convert.ToInt32(value));

		public EnumNotSupportedException(Type type, int value, string? message = null)
			: base(message ?? DEFAULT_MESSAGE)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			Value = value;
		}

		public Type Type { get; }
		public int Value { get; }
	}
}
