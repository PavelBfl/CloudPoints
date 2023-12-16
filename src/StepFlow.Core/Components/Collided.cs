using StepFlow.Intersection;

namespace StepFlow.Core.Components
{
	public sealed class Collided : ComponentBase
	{
		private void SetShape(ref IRefCounter<ShapeBase>? field, IRefCounter<ShapeBase>? value)
		{
			if (field is { })
			{
				field.RemoveRef();
				field.Value.Attached = null;
			}

			field = value;

			if (field is { })
			{
				field.AddRef();
				field.Value.Attached = this;
			}
		}

		private IRefCounter<ShapeBase>? current;

		public IRefCounter<ShapeBase>? Current { get => current; set => SetShape(ref current, value); }

		private IRefCounter<ShapeBase>? next;

		public IRefCounter<ShapeBase>? Next { get => next; set => SetShape(ref next, value); }

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }
	}
}
