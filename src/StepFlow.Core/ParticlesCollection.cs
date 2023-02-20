using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class ParticlesCollection : ICollection<Particle>, IReadOnlyCollection<Particle>
	{
		public ParticlesCollection(World owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		private World Owner { get; }

		private HashSet<Particle> Items { get; } = new HashSet<Particle>();

		public void Add(Particle item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (ReferenceEquals(Owner, item.Owner))
			{
				throw InvalidCoreException.CreateAddAlreadyExistsParticle();
			}

			if (item.Owner is { })
			{
				throw InvalidCoreException.CreateAddParticleBelongOtherOwner();
			}

			item.Owner = Owner;
		}

		internal void AddForce(Particle item) => Items.Add(item);

		public void Clear()
		{
			foreach (var item in Items.ToArray())
			{
				item.Owner = null;
			}
		}

		public bool Contains(Particle item) => Items.Contains(item);

		public void CopyTo(Particle[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

		public IEnumerator<Particle> GetEnumerator() => Items.GetEnumerator();

		public bool Remove(Particle item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (item.Owner is null)
			{
				throw InvalidCoreException.CreateParticleWithoutOwner();
			}

			if (!ReferenceEquals(Owner, item.Owner))
			{
				throw InvalidCoreException.CreateAddAlreadyExistsParticle();
			}

			item.Owner = null;
			return true;
		}

		internal void RemoveForce(Particle item) => Items.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
