using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Exceptions;
using StepFlow.ViewModel.Services;

namespace StepFlow.ViewModel
{
	public class ParticlesCollectionVm : CollectionWrapperObserver<ParticleVm, GamePlay.IParticle>
	{
		public ParticlesCollectionVm(ContextVm owner)
			: base()
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
	}
}
