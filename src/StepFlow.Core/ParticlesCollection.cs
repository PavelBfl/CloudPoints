using System;
using System.Collections.ObjectModel;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class ParticlesCollection : Collection<Particle>
	{
		public ParticlesCollection(World owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private World Owner { get; }

		private void CheckNewItem(Particle item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (ReferenceEquals(Owner, item))
			{
				throw InvalidCoreException.CreateAddAlreadyExistsParticle();
			}

			if (item.Owner is { })
			{
				throw InvalidCoreException.CreateAddParticleBelongOtherOwner();
			}
		}

		protected override void ClearItems()
		{
			foreach (var item in this)
			{
				item.Owner = null;
			}

			base.ClearItems();
		}

		protected override void InsertItem(int index, Particle item)
		{
			CheckNewItem(item);

			item.Owner = Owner;

			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			this[index].Owner = null;

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Particle item)
		{
			CheckNewItem(item);

			item.Owner = Owner;
			base[index].Owner = null;

			base.SetItem(index, item);
		}
	}
}
