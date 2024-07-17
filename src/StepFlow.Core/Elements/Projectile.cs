using System.Collections.Generic;
using System.Linq;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Projectile(IContext context)
			: base(context)
		{
		}

		public Projectile(IContext context, ProjectileDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Damage = original.Damage;
			Reusable = original.Reusable;
			Immunity.AddUniqueRange(original.Immunity.Select(x => x.ToSubject(context)));
		}

		public Damage Damage { get; set; }

		public ReusableKind Reusable { get; set; }

		public ICollection<Subject> Immunity { get; } = new HashSet<Subject>();

		public override SubjectDto ToDto()
		{
			var result = new ProjectileDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(ProjectileDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Damage = Damage;
			container.Reusable = Reusable;
			container.Immunity.AddRange(Immunity.Select(x => x.ToDto()));
		}
	}
}
