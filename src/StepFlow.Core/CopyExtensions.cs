using System;
using System.Collections.Generic;
using StepFlow.Core.Elements;
using StepFlow.Core.Exceptions;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core
{
	public static class CopyExtensions
	{
		public static TrackChange ToTrackChange(this TrackChangeDto original, IContext context) => new TrackChange(context, original);

		public static TrackUnit ToTrackUnit(this TrackUnitDto original, IContext context) => new TrackUnit(context, original);

		public static TrackBuilder ToTrackBuilder(this TrackBuilderDto original, IContext context) => new TrackBuilder(context, original);

		public static State ToState(this StateDto original, IContext context) => new State(context, original);

		public static Enemy ToEnemy(this EnemyDto original, IContext context) => new Enemy(context, original);

		public static Item ToItem(this ItemDto original, IContext context) => new Item(context, original);

		public static Obstruction ToObstruction(this ObstructionDto original, IContext context) => new Obstruction(context, original);

		public static Place ToPlace(this PlaceDto original, IContext context) => new Place(context, original);

		public static Wormhole ToPlaygroundSwitch(this WormholeDto original, IContext context) => new Wormhole(context, original);

		public static PlayerCharacter ToPlayerCharacter(this PlayerCharacterDto original, IContext context) => new PlayerCharacter(context, original);

		public static Projectile ToProjectile(this ProjectileDto original, IContext context) => new Projectile(context, original);

		public static Track ToTrack(this TrackDto original, IContext context) => new Track(context, original);

		public static ElementBase ToElementBase(this ElementBaseDto original, IContext context) => original switch
		{
			MaterialDto material => ToMaterial(material, context),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

		public static Material ToMaterial(this MaterialDto original, IContext context) => original switch
		{
			EnemyDto enemy => ToEnemy(enemy, context),
			ItemDto item => ToItem(item, context),
			ObstructionDto obstruction => ToObstruction(obstruction, context),
			PlaceDto place => ToPlace(place, context),
			PlayerCharacterDto player => ToPlayerCharacter(player, context),
			ProjectileDto projectile => ToProjectile(projectile, context),
			WormholeDto playgroundSwitch => ToPlaygroundSwitch(playgroundSwitch, context),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

		public static Subject ToSubject(this SubjectDto original, IContext context) => original switch
		{
			ElementBaseDto element => ToElementBase(element, context),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

		public static void Reset<T>(this ICollection<T> container, IEnumerable<T>? items)
		{
			container.Clear();

			if (items is { })
			{
				foreach (var item in items)
				{
					container.Add(item);
				}
			}
		}

		public static void AddRange<T>(this ICollection<T> container, IEnumerable<T> items)
		{
			if (container is null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (var value in items)
			{
				container.Add(value);
			}
		}

		public static void AddUniqueRange<T>(this ICollection<T> container, IEnumerable<T> items)
		{
			if (container is null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (container is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			var counter = 0;
			foreach (var item in items)
			{
				container.Add(item);
				counter++;

				if (counter != container.Count)
				{
					throw ExceptionBuilder.CreateNonuniqueItemForCopy();
				}
			}
		}

		internal static void ThrowIfArgumentNull(object? value, string name)
		{
			if (value is null)
			{
				throw new ArgumentNullException(name);
			}
		}

		internal static void ThrowIfOriginalNull(object? original) => ThrowIfArgumentNull(original, nameof(original));

		internal static void ThrowIfContextNull(IContext context) => ThrowIfArgumentNull(context, nameof(context));
	}
}
