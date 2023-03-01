using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using StepFlow.Core;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class ParticlesCollectionVm : INotifyCollectionChanged
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

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public bool Contains(Particle particle) => ByModel.ContainsKey(particle);

		internal void AddForce(IParticleVm particleVm)
		{
			ByModel.Add(particleVm.Source, particleVm);
			ByViewModel.Add(particleVm, particleVm.Source);

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		internal void RemoveForce(IParticleVm particleVm)
		{
			if (ByViewModel.Remove(particleVm, out var particle))
			{
				if (!ByModel.Remove(particle))
				{
					throw InvalidViewModelException.CreateInvalidMatchPairs();
				}

				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}
	}
}
