using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Components;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public abstract class Material : ElementBase
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 1000;

		public Material(IContext context)
			: base(context)
		{
		}

		public Material(IContext context, MaterialDto original)
			: base(context, original)
		{
			Strength = original.Strength;
			Body = original.Body?.ToCollided(context);
			Speed = original.Speed;
			Weight = original.Weight;
			Course = original.Course;
			CollisionBehavior = original.CollisionBehavior;
			States.AddUniqueRange(original.States.Select(x => x.ToState(Context)));
			Track = original.Track?.ToTrackBuilder(Context);
		}

		public Scale Strength { get; set; }

		private Collided? body;

		public Collided? Body { get => body; set => SetComponent(ref body, value); }

		public Collided GetBodyRequired() => NullValidate.PropertyRequired(Body, nameof(Body));

		public int Speed { get; set; }

		public int Weight { get; set; }

		public Vector2 Course { get; set; }

		public CollisionBehavior CollisionBehavior { get; set; }

		public ICollection<State> States { get; } = new HashSet<State>();

		public TrackBuilder? Track { get; set; }

		public void CopyTo(MaterialDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Strength = Strength;
			container.Body = (CollidedDto?)Body?.ToDto();
			container.Speed = Speed;
			container.Weight = Weight;
			container.Course = Course;
			container.CollisionBehavior = CollisionBehavior;
		}

		public virtual void Begin()
		{
			Body?.Begin();
		}

		public virtual void End()
		{
			Body?.End();
		}
	}
}
