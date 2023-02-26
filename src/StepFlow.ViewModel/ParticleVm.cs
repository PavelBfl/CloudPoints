using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class ParticleVm<T> : WrapperVm<T>, IParticleVm
		where T : Particle?
	{
		public ParticleVm(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			WorldProvider = ServiceProvider.GetRequiredService<IWorldProvider>();
		}

		private IWorldProvider WorldProvider { get; }

		public WorldVm? Owner { get; private set; }

		Particle? IParticleVm.Source => Source;

		[AllowNull]
		[MaybeNull]
		internal override T Source
		{
			get => base.Source;
			set
			{
				if (Source != value)
				{
					if (Source is { })
					{
						Owner.PropertyRequired(nameof(Owner)).Particles.RemoveForce(this);
						Owner = null;
					}

					base.Source = value;

					if (Source is { })
					{
						Owner = WorldProvider[Source.OwnerRequired];
						Owner.Particles.AddForce(Source, this);
					}
				}
			}
		}
	}
}
