using System.Runtime.CompilerServices;
using StepFlow.Core.Exceptions;
using StepFlow.Domains;

namespace StepFlow.Core
{
	public abstract class Subject
	{
		public static T PropertyRequired<T>(T? value, [CallerMemberName] string? propertyName = null)
			where T : class
			=> value ?? throw ExceptionBuilder.CreatePropertyIsNull(propertyName);

		public static T PropertyRequired<T>(T? value, [CallerMemberName] string? propertyName = null)
			where T : struct
			=> value ?? throw ExceptionBuilder.CreatePropertyIsNull(propertyName);

		public Subject(IContext context)
		{
			CopyExtensions.ThrowIfArgumentNull(context, nameof(context));

			Context = context;
		}

		public Subject(IContext context, SubjectDto original)
			: this(context)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Name = original.Name;
		}

		public IContext Context { get; }

		public string? Name { get; set; }

		public void CopyTo(SubjectDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			container.Name = Name;
		}

		public abstract SubjectDto ToDto();
	}
}
