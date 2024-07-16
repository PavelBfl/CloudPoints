using System.Collections.Generic;
using System.Linq;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Projectile()
		{
		}

		public Projectile(ProjectileDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			Damage = original.Damage;
			Reusable = original.Reusable;
			Immunity.AddUniqueRange(original.Immunity.Select(CopyExtensions.ToSubject));
		}

		public Damage Damage { get; set; }

		public ReusableKind Reusable { get; set; }

		public ICollection<Subject> Immunity { get; } = new HashSet<Subject>();
	}
}
