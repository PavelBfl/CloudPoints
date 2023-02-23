using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.Core;

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
						Owner.Particles.RemoveForce(this);
						Owner = null;
					}

					base.Source = value;

					if (Source is { })
					{
						Owner = WorldProvider.GetWorld(Source.OwnerSafe);
						Owner.Particles.AddForce(Source, this);
					}
				}
			}
		}
	}
}
