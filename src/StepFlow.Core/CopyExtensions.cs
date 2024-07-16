using System;
using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Exceptions;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core
{
	internal static class CopyExtensions
	{
		public static Collided ToCollided(this CollidedDto original) => new Collided(original);

		public static ComponentBase ToComponent(this ComponentBaseDto original) => original switch
		{
			CollidedDto collided => ToCollided(collided),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

		public static TrackChange ToTrackChange(this TrackChangeDto original) => new TrackChange(original);

		public static TrackUnit ToTrackUnit(this TrackUnitDto original) => new TrackUnit(original);

		public static TrackBuilder ToTrackBuilder(this TrackBuilderDto original) => new TrackBuilder(original);

		public static State ToState(this StateDto original) => new State(original);

		public static Enemy ToEnemy(this EnemyDto original) => new Enemy(original);

		public static Item ToItem(this ItemDto original) => new Item(original);

		public static Obstruction ToObstruction(this ObstructionDto original) => new Obstruction(original);

		public static Place ToPlace(this PlaceDto original) => new Place(original);

		public static PlayerCharacter ToPlayerCharacter(this PlayerCharacterDto original) => new PlayerCharacter(original);

		public static Projectile ToProjectile(this ProjectileDto original) => new Projectile(original);

		public static Track ToTrack(this TrackDto original) => new Track(original);

		public static ElementBase ToElementBase(this ElementBaseDto original) => original switch
		{
			MaterialDto material => ToMaterial(material),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

		public static Material ToMaterial(this MaterialDto original) => original switch
		{
			EnemyDto enemy => new Enemy(enemy),
			ItemDto item => new Item(item),
			ObstructionDto obstruction => new Obstruction(obstruction),
			PlaceDto place => new Place(place),
			PlayerCharacterDto player => new PlayerCharacter(player),
			ProjectileDto projectile => new Projectile(projectile),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

		public static Subject ToSubject(this SubjectDto original) => original switch
		{
			ComponentBaseDto component => ToComponent(component),
			ElementBaseDto element => ToElementBase(element),
			null => throw new ArgumentNullException(nameof(original)),
			_ => throw ExceptionBuilder.CreateUnknownTypeForCopy(original.GetType()),
		};

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
	}
}
