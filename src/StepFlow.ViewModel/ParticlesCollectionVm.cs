using System;
using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class ParticlesCollectionVm
	{
		public ParticlesCollectionVm(WorldVm owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public WorldVm Owner { get; }

		public IParticleVm this[Particle key]
		{
			get
			{
				if (key is null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return ByModel[key];
			}
		}

		public Particle this[IParticleVm key]
		{
			get
			{
				if (key is null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return ByViewModel[key];
			}
		}

		private Dictionary<Particle, IParticleVm> ByModel { get; } = new Dictionary<Particle, IParticleVm>();

		private Dictionary<IParticleVm, Particle> ByViewModel { get; } = new Dictionary<IParticleVm, Particle>();

		public IReadOnlyCollection<Particle> Models => ByModel.Keys;
		public IReadOnlyCollection<IParticleVm> ViewsModels => ByViewModel.Keys;

		public int Count => ByModel.Count;

		internal void AddForce(Particle particle, IParticleVm particleVm)
		{
			ByModel.Add(particle, particleVm);
			ByViewModel.Add(particleVm, particle);
		}

		internal void RemoveForce(IParticleVm particleVm)
		{
			if (ByViewModel.Remove(particleVm, out var particle))
			{
				if (!ByModel.Remove(particle))
				{
					throw InvalidViewModelException.CreateInvalidMatchPairs();
				}
			}
		}
	}
}
