using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Exceptions;
using StepFlow.ViewModel.Services;

namespace StepFlow.ViewModel
{
	public class ParticlesCollectionVm : CollectionWrapperObserver<ParticleVm, GamePlay.IParticle>
	{
		public ParticlesCollectionVm(ContextVm owner)
			: base(new Collection(owner.Source.World.Particles))
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			WrapperProvider = Owner.ServiceProvider.GetRequiredService<IWrapperProvider>();
		}

		private ContextVm Owner { get; }

		private IWrapperProvider WrapperProvider { get; }

		protected override ParticleVm CreateObserver(GamePlay.IParticle observable)
		{
			if (WrapperProvider.TryGetViewModel(observable, out var result))
			{
				return (ParticleVm)result;
			}
			else
			{
				return observable switch
				{
					GamePlay.Piece piece => new PieceVm(Owner, piece),
					GamePlay.Node node => new NodeVm(Owner, node),
					_ => throw new InvalidViewModelException(),
				};
			}
		}

		private class Collection : ICollection<GamePlay.IParticle>
		{
			public Collection(ICollection<Core.Particle> target)
			{
				Target = target ?? throw new ArgumentNullException(nameof(target));
			}

			public ICollection<Core.Particle> Target { get; }

			public int Count => Target.Count;

			public bool IsReadOnly => Target.IsReadOnly;

			public void Add(GamePlay.IParticle item) => Target.Add((Core.Particle)item);

			public void Clear() => Target.Clear();

			public bool Contains(GamePlay.IParticle item) => Target.Contains((Core.Particle)item);

			public void CopyTo(GamePlay.IParticle[] array, int arrayIndex) => Target.Cast<Core.Particle>().ToArray().CopyTo(array, arrayIndex);

			public IEnumerator<GamePlay.IParticle> GetEnumerator() => Target.Cast<GamePlay.IParticle>().GetEnumerator();

			public bool Remove(GamePlay.IParticle item) => Target.Remove((Core.Particle)item);

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
