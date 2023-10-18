using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using MoonSharp.Interpreter;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Components;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public static class Components
	{
		public static class Types
		{
			public const string COLLIDED = "CollidedType";
			public const string SCALE = "ScaleType";
			public const string SCHEDULER = "SchedulerType";
			public const string DAMAGE= "DamageType";
			public const string PROJECTILE_SETTINGS = "ProjectileSettingsType";
			public const string HANDLER = "Handler";
			public const string SET_COURSE = "CourseHandler";
			public const string SENTRY_GUN = "SentryGun";
		}

		public static class Names
		{
			public const string STRENGTH = "Strength";
			public const string COLLIDED = "Collided";
			public const string DAMAGE = "Damage";
			public const string MAIN_SCHEDULER = "MainScheduler";

			public const string PROJECTILE_SETTINGS = "ProjectileSettings";
			public const string PROJECTILE_SETTINGS_SET = "ProjectileSettingsSet";

			public const string VISION = "Vision";
		}
	}

	public class PlayMaster
	{
		private const string TAKE_STEP_NAME = nameof(TakeStep);
		private const string ENUMERATE_NAME = "Enumerate";

		private const string TAKE_STEP_CALL = TAKE_STEP_NAME + "()";

		public const string FIRE_DAMAGE = "Fire";
		public const string POISON_DAMAGET = "Poison";

		public PlayMaster()
		{
			InitLua();
		}

		public IAxis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public long Time { get; private set; }

		public Playground Playground { get; } = new Playground();

		#region Proxy
		public IPlaygroundProxy GetPlaygroundProxy() => CreateProxy(Playground);

		[return: NotNullIfNotNull("obj")]
		internal object? CreateProxy(object? obj) => obj switch
		{
			Playground playground => CreateProxy(playground),
			Subject subject => CreateProxy(subject),
			Collided collided => CreateProxy(collided),
			Damage projectile => CreateProxy(projectile),
			Scale scale => CreateProxy(scale),
			Scheduled scheduled => CreateProxy(scheduled),
			Cell cell => CreateProxy(cell),
			Bordered bordered => CreateProxy(bordered),
			ProjectileSettings projectileSettings => CreateProxy(projectileSettings),
			SetCourse setCourse => new SetCourseProxy(this, setCourse),
			Handler handler => new HandlerProxy(this, handler),
			SentryGun sentryGun => new SentryGunProxy(this, sentryGun),
			null => null,
			_ => throw new InvalidOperationException(),
		};

		[return: NotNullIfNotNull("obj")]
		internal ICellProxy? CreateProxy(Cell? obj) => obj is null ? null : new CellProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IBorderedProxy? CreateProxy(Bordered? obj) => obj is null ? null : new BorderedProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IPlaygroundProxy? CreateProxy(Playground? obj) => obj is null ? null : new PlaygroundProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal ISubjectProxy? CreateProxy(Subject? obj) => obj is null ? null : new SubjectProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal ICollidedProxy? CreateProxy(Collided? obj) => obj is null ? null : new CollidedProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IDamageProxy? CreateProxy(Damage? obj) => obj is null ? null : new DamageProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IScaleProxy? CreateProxy(Scale? obj) => obj is null ? null : new ScaleProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IScheduledProxy? CreateProxy(Scheduled? obj) => obj is null ? null : new ScheduledProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IProjectileSettingsProxy? CreateProxy(ProjectileSettings? obj) => obj is null ? null : new ProjectileSettingsProxy(this, obj);
		#endregion

		#region Handlers
		public const string COLLISION_HANDLER = "Collision";
		public const string PROJECTILE_BUILDER_HANDLER = "ProjectileBuilder";
		public const string REMOVE_COMPONENT_HANDLER = "RemoveComponent";
		public const string REMOVE_SUBJECT_HANDLER = "RemoveSubject";
		public const string SCALE_EMPTY_HANDLER = "ScaleEmptyHandler";
		public const string SET_COURSE_HANDLER = "SetCourseHandler";
		public const string SET_DAMAGE_HANDLER = "SetDamage";
		public const string SENTRY_GUN_REACT_HANDLER = "SentryGunReact";

		public void CallHandler(IHandlerProxy main, IComponentProxy component)
		{
			switch (main.Reference)
			{
				case COLLISION_HANDLER:
					CollisionHandler(main, component);
					break;
				case PROJECTILE_BUILDER_HANDLER:
					ProjectileBuilder(main, component);
					break;
				case REMOVE_COMPONENT_HANDLER:
					RemoveComponent(main, component);
					break;
				case REMOVE_SUBJECT_HANDLER:
					RemoveSubject(main, component);
					break;
				case SCALE_EMPTY_HANDLER:
					ScaleEmptyHandler(main, component);
					break;
				case SET_COURSE_HANDLER:
					SetCourse(main, component);
					break;
				case SET_DAMAGE_HANDLER:
					SetDamage(main, component);
					break;
				case SENTRY_GUN_REACT_HANDLER:
					SentryGunReact(main, component);
					break;
				default: throw new InvalidOperationException();
			}
		}

		private static void CollisionHandler(IComponentProxy main, IComponentProxy component)
		{
			if (main.Subject.GetComponent(Master.Components.Names.STRENGTH) is IScaleProxy scale &&
				component.Subject.GetComponent(Master.Components.Names.DAMAGE) is IDamageProxy damage)
			{
				if (!damage.Kind.Any())
				{
					scale.Add(-damage.Value);
				}
				else
				{
					if (damage.Kind.Contains(PlayMaster.FIRE_DAMAGE))
					{
						scale.Add(-damage.Value * 2);
					}

					if (damage.Kind.Contains(PlayMaster.POISON_DAMAGET))
					{
						var poisonSubject = main.Subject.Playground.CreateSubject();
						var damageSubject = (IDamageProxy)poisonSubject.AddComponent(Master.Components.Types.DAMAGE, Master.Components.Names.DAMAGE);
						damageSubject.Value = damage.Value / 2;
						damageSubject.Kind.Add(PlayMaster.POISON_DAMAGET);
						var setDamageHandler = poisonSubject.AddHandler(PlayMaster.SET_DAMAGE_HANDLER);
						var removeSubjectHandler = poisonSubject.AddHandler(PlayMaster.REMOVE_SUBJECT_HANDLER, true);

						var poisonScheduler = (IScheduledProxy)main.Subject.AddComponent(Master.Components.Types.SCHEDULER);
						for (var i = 0; i < 5; i++)
						{
							poisonScheduler.Add(60, setDamageHandler);
						}
						poisonScheduler.Add(0, removeSubjectHandler);
					}
				}
			}

			if (main.Subject.GetComponent(Master.Components.Names.COLLIDED) is ICollidedProxy mainCollided &&
				component.Subject.GetComponent(Master.Components.Names.COLLIDED) is ICollidedProxy otherCollided)
			{
				if (otherCollided.IsRigid)
				{
					mainCollided.Break();
				}
			}

			if (main.Subject.GetComponent(Master.Components.Names.PROJECTILE_SETTINGS) is IProjectileSettingsProxy mainSettings &&
				component.Subject.GetComponent(Master.Components.Names.PROJECTILE_SETTINGS_SET) is IProjectileSettingsProxy otherSettings)
			{
				foreach (var kind in otherSettings.Kind)
				{
					mainSettings.Kind.Add(kind);
				}
			}
		}

		private static Point GetPivot(Rectangle rectangle, Course position) => position switch
		{
			Course.Left => new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2),
			Course.LeftTop => new Point(rectangle.Left, rectangle.Top),
			Course.Top => new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top),
			Course.RightTop => new Point(rectangle.Right, rectangle.Top),
			Course.Right => new Point(rectangle.Right, rectangle.Top + rectangle.Height / 2),
			Course.RightBottom => new Point(rectangle.Right, rectangle.Bottom),
			Course.Bottom => new Point(rectangle.Left + rectangle.Width / 2, rectangle.Bottom),
			Course.LeftBottom => new Point(rectangle.Left, rectangle.Bottom),
			_ => throw EnumNotSupportedException.Create(position),
		};

		private static Rectangle CreateRectangle(Course pivot, Point position, Size size) => pivot switch
		{
			Course.Left => new Rectangle(new Point(position.X, position.Y - size.Width / 2), size),
			Course.LeftTop => new Rectangle(position, size),
			Course.Top => new Rectangle(new Point(position.X - size.Width / 2, position.Y), size),
			Course.RightTop => new Rectangle(new Point(position.X - size.Width, position.Y), size),
			Course.Right => new Rectangle(new Point(position.X - size.Width, position.Y - size.Height / 2), size),
			Course.RightBottom => new Rectangle(new Point(position.X - size.Width, position.Y - size.Height), size),
			Course.Bottom => new Rectangle(new Point(position.X - size.Width / 2, position.Y - size.Height), size),
			Course.LeftBottom => new Rectangle(new Point(position.X, position.Y - size.Height), size),
			_ => throw EnumNotSupportedException.Create(pivot),
		};

		private static void ProjectileBuilder(IComponentProxy main, IComponentProxy component)
		{
			var ownerCollided = main.Subject.GetComponent(Master.Components.Names.COLLIDED);
			if (ownerCollided is ICollidedProxy { Current: { } current })
			{
				var projectileSettings = (IProjectileSettingsProxy)main.Subject.GetComponentRequired(Master.Components.Names.PROJECTILE_SETTINGS);

				var playground = main.Subject.Playground;

				var subject = playground.CreateSubject();
				playground.Subjects.Add(subject);
				var collided = (ICollidedProxy)subject.AddComponent(Components.Types.COLLIDED, Components.Names.COLLIDED);

				var bordered = playground.CreateBordered();

				var pivot = GetPivot(current.Target.Border, projectileSettings.Course);
				var projectileBorder = CreateRectangle(projectileSettings.Course.Invert(), pivot, new Size(projectileSettings.Size, projectileSettings.Size));
				projectileBorder.Offset(projectileSettings.Course.ToOffset());

				bordered.AddCell(projectileBorder);
				collided.Current = bordered;
				var removeSubjectHandler = subject.AddHandler(PlayMaster.REMOVE_SUBJECT_HANDLER);
				collided.Collision.Add(removeSubjectHandler);

				var projectile = (IDamageProxy)subject.AddComponent(Master.Components.Types.DAMAGE, Master.Components.Names.DAMAGE);
				projectile.Value = projectileSettings.Damage;
				foreach (var kind in projectileSettings.Kind)
				{
					projectile.Kind.Add(kind);
				}

				var scheduler = (IScheduledProxy)subject.AddComponent(Master.Components.Types.SCHEDULER);
				for (var i = 0; i < 100; i++)
				{
					scheduler.SetCourse(projectileSettings.Course);
				}
				var removeSubject = subject.AddHandler(PlayMaster.REMOVE_SUBJECT_HANDLER, true);
				scheduler.Add(0, removeSubject);
			}
		}

		private static ISubjectProxy CreateProjectile(ISubjectProxy owner)
		{
			var result = owner.Playground.CreateSubject();

		}

		private static void RemoveComponent(IComponentProxy main, IComponentProxy component)
		{
			component.Subject.RemoveComponent(component);
		}

		private static void RemoveSubject(IComponentProxy main, IComponentProxy component)
		{
			main.Subject.Playground.Subjects.Remove(main.Subject);
		}

		private static void ScaleEmptyHandler(IComponentProxy main, IComponentProxy component)
		{
			if (((IScaleProxy)component).Value <= 0)
			{
				main.Subject.Playground.Subjects.Remove(main.Subject);
			}
		}

		private static void SetCourse(IComponentProxy main, IComponentProxy component)
		{
			if (component.Subject.GetComponent(Master.Components.Names.COLLIDED) is ICollidedProxy collided)
			{
				var setCourse = main.Subject.GetComponents().OfType<ISetCourseProxy>().Single();
				var offset = setCourse.Course.ToOffset();
				collided.Offset(offset);
			}
		}

		private static void SetDamage(IComponentProxy main, IComponentProxy component)
		{
			var damage = (IDamageProxy)main.Subject.GetComponentRequired(Master.Components.Names.DAMAGE);

			if (component.Subject.GetComponent(Master.Components.Names.STRENGTH) is IScaleProxy scale)
			{
				scale.Add(-damage.Value);
			}
		}

		private static void SentryGunReact(IComponentProxy main, IComponentProxy component)
		{
			if (main.Subject == component.Subject)
			{
				return;
			}

			var projectile = main.Subject.Playground.CreateSubject();

		}
		#endregion

		public IComponent CreateComponent(string componentType)
		{
			if (componentType is null)
			{
				throw new ArgumentNullException(nameof(componentType));
			}

			return componentType switch
			{
				Components.Types.COLLIDED => new Collided(Playground),
				Components.Types.SCALE => new Scale(Playground),
				Components.Types.SCHEDULER => new Scheduled(Playground),
				Components.Types.DAMAGE => new Damage(Playground),
				Components.Types.PROJECTILE_SETTINGS => new ProjectileSettings(Playground),
				Components.Types.HANDLER => new Handler(Playground),
				Components.Types.SET_COURSE => new SetCourse(Playground),
				Components.Types.SENTRY_GUN => new SentryGun(Playground),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void CollisionHandle(ISubjectProxy main, ISubjectProxy other)
		{
			var collided = (ICollidedProxy)main.GetComponentRequired(Components.Names.COLLIDED);
			var otherCollided = (ICollidedProxy)other.GetComponentRequired(Components.Names.COLLIDED);
			foreach (var handler in collided.Collision.Cast<IHandlerProxy>())
			{
				handler.Handle(otherCollided);
			}
		}

		private void TakeStepInner()
		{
			var playground = GetPlaygroundProxy();

			foreach (var collision in playground.GetCollision().ToArray())
			{
				CollisionHandle(collision.Item1, collision.Item2);
				CollisionHandle(collision.Item2, collision.Item1);
			}

			foreach (var collided in playground.Subjects
				.ToArray()
				.Select(x => x.GetComponent(Components.Names.COLLIDED))
				.OfType<ICollidedProxy>()
			)
			{
				collided.Move();
			}

			Time++;

			foreach (var scheduler in playground.Subjects
				.ToArray()
				.SelectMany(x => x.GetComponents())
				.OfType<IScheduledProxy>()
			)
			{
				scheduler.TryDequeue();
			}
		}

		private static void RegisterList<T>()
		{
			UserData.RegisterType<T>();

			UserData.RegisterType<IEnumerable<T>>();
			UserData.RegisterType<IEnumerator<T>>();
			UserData.RegisterType<IReadOnlyList<T>>();
			UserData.RegisterType<IReadOnlyCollection<T>>();
			UserData.RegisterType<IList<T>>();
			UserData.RegisterType<ICollection<T>>();
		}

		private void InitLua()
		{
			UserData.RegisterType<IEnumerator>();
			UserData.RegisterType<Rectangle>();
			UserData.RegisterType<Point>();

			RegisterList<(Subject, Subject)>();
			RegisterList<ICellProxy>();
			RegisterList<IBorderedProxy>();

			RegisterList<IComponentController>();
			RegisterList<IComponentProxy>();

			RegisterList<IPlaygroundProxy>();
			RegisterList<ISubjectProxy>();
			RegisterList<ICollidedProxy>();
			RegisterList<IDamageProxy>();
			RegisterList<IScaleProxy>();
			RegisterList<IScheduledProxy>();
			RegisterList<IProjectileSettingsProxy>();
		}

		public static DynValue Enumerate(ScriptExecutionContext context, CallbackArguments arguments)
		{
			var dynEnumerable = arguments[0];
			var enumerable = (IEnumerable)dynEnumerable.UserData.Object;

			return DynValue.NewTuple(
				DynValue.NewCallback(EnumerateIteration),
				dynEnumerable,
				DynValue.FromObject(context.GetScript(), enumerable.GetEnumerator())
			);

			DynValue EnumerateIteration(ScriptExecutionContext context, CallbackArguments arguments)
			{
				var dynEnumerable = arguments[0];
				var enumerable = (IEnumerable)dynEnumerable.UserData.Object;

				var dynEnumerator = arguments[1];
				var enumerator = (IEnumerator)dynEnumerator.UserData.Object;

				if (enumerator.MoveNext())
				{
					return DynValue.NewTuple(
						dynEnumerator,
						DynValue.FromObject(context.GetScript(), enumerator.Current)
					);
				}
				else
				{
					return DynValue.Nil;
				}
			}
		}

		public void Execute(string scriptText)
		{
			if (scriptText == TAKE_STEP_CALL)
			{
				TakeStepInner();
			}
			else
			{
				var script = new Script();

				script.Globals["playground"] = GetPlaygroundProxy();

				script.Globals[TAKE_STEP_NAME] = (Action)TakeStepInner;
				script.Globals.Set(ENUMERATE_NAME, DynValue.NewCallback(Enumerate));
				script.DoString(scriptText); 
			}
		}
	}
}
