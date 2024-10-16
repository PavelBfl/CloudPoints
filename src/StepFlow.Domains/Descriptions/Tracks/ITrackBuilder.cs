using StepFlow.Common;
using StepFlow.Domains.Elements;

namespace StepFlow.Domains.Descriptions.Tracks
{
	public interface ITrackBuilder : ISubject
	{
		public static void CopyTo(ITrackBuilder source, ITrackBuilder destination)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));
			NullValidate.ThrowIfArgumentNull(destination, nameof(destination));

			ISubject.CopyTo(source, destination);


			destination.Cooldown = source.Cooldown;
			PropertyCopyTo(source.GetClonerChange(), destination.GetClonerChange());
		}

		IClonerProperty<ITrackChange> GetClonerChange();

		Scale Cooldown { get; set; }
	}
}
