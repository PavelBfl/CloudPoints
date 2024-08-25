using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;
using StepFlow.Domains;
using StepFlow.Domains.Elements;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public const int CELL_SIZE_DEFAULT = 50;

		public Playground(IContext context)
			: base(context)
		{
		}

		public Playground(IContext context, PlaygroundDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Items.AddUniqueRange(original.Items.Select(x => x.ToMaterial(Context)));
		}

		public PlayerCharacter GetPlayerCharacterRequired() => Items.OfType<PlayerCharacter>().Single();

		public ICollection<Material> Items { get; } = new ItemsCollection();

		public override SubjectDto ToDto()
		{
			var result = new PlaygroundDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(PlaygroundDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Items.AddRange(Items.Select(x => x.ToDto()).Cast<MaterialDto>());
		}

		private sealed class ItemsCollection : ICollection<Material>
		{
			public int Count => Source.Count;

			public bool IsReadOnly => false;

			private HashSet<Material> Source { get; } = new HashSet<Material>();

			public void Add(Material item)
			{
				if (Source.Add(item))
				{
					item.Enable();
				}
			}

			public void Clear()
			{
				foreach (var item in this)
				{
					item.Disable();
				}

				Source.Clear();
			}

			public bool Contains(Material item) => Source.Contains(item);

			public void CopyTo(Material[] array, int arrayIndex) => Source.CopyTo(array, arrayIndex);

			public IEnumerator<Material> GetEnumerator() => Source.GetEnumerator();

			public bool Remove(Material item)
			{
				var removed = Source.Remove(item);
				if (removed)
				{
					item.Disable();
				}

				return removed;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
