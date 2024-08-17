using StepFlow.Core.Exceptions;
using StepFlow.Domains;

namespace StepFlow.Core
{
	public abstract class Subject
	{
		public static T PropertyCopyRequired<T>(T? value, string propertyName)
			where T : class
			=> value ?? throw ExceptionBuilder.CreateCopiedPropertyIsNull(propertyName);

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
