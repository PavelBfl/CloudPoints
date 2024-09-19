using StepFlow.Common;
using StepFlow.Intersection;

namespace StepFlow.Core.Elements
{
	public sealed class CollidedAttached
	{
		public CollidedAttached(string name, Material material)
		{
			NullValidate.ThrowIfArgumentNull(name, nameof(name));
			NullValidate.ThrowIfArgumentNull(material, nameof(material));

			Name = name;
			Material = material;
		}

		public string Name { get; }

		public Material Material { get; }

		public Shape? GetShape() => Material.GetShape(Name);

		public override string ToString() => Name + ": " + GetShape();
	}
}
