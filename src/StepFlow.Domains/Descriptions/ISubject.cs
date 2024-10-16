using System.Collections.Generic;
using StepFlow.Common;

namespace StepFlow.Domains.Descriptions
{
	public interface ISubject : IClonerTo<ISubject>
	{
		public static void PropertyCopyTo<T>(IClonerProperty<T> source, IClonerProperty<T> destination)
			where T : IClonerTo<T>
		{
			if (source.IsEmpty())
			{
				destination.Clear();
			}
			else
			{
				source.Value.CloneTo(destination.GetOrCreate());
			}
		}

		public static void CollectionReset<T>(ICollection<T> source, ICollection<T> destination)
		{
			destination.Clear();
			foreach (var item in source)
			{
				destination.Add(item);
			}
		}

		public static void CopyTo(ISubject source, ISubject destination)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));
			NullValidate.ThrowIfArgumentNull(destination, nameof(destination));

			destination.Name = source.Name;
		}

		void IClonerTo<ISubject>.CloneTo(ISubject container) => CopyTo(this, container);

		string? Name { get; set; }
	}
}
