using StepFlow.Intersection;

namespace StepFlow.Core.Components
{
	public sealed class Collided : ComponentBase
	{
		private void SetShape(ref ShapeBase? field, ShapeBase? value)
		{
			if (field is { })
			{
				field.Context.Remove(field);
				field.Attached = null;
			}

			field = value;

			if (field is { })
			{
				field.Context.Add(field);
				field.Attached = this;
			}
		}

		private ShapeBase? current;

		public ShapeBase? Current { get => current; set => SetShape(ref current, value); }

		public ShapeBase GetCurrentRequired() => PropertyRequired(Current, nameof(Current));

		private ShapeBase? next;

		public ShapeBase? Next { get => next; set => SetShape(ref next, value); }

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }
	}
}
