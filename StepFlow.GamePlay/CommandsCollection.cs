using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StepFlow.Core;

namespace StepFlow.GamePlay
{
	internal class CommandsCollection : IReadOnlyDictionary<Particle?, IReadOnlyList<Command>>
	{
		public IReadOnlyList<Command> this[Particle? key] => key is null ? CommonCommands : Commands[key];

		public IEnumerable<Particle?> Keys => Commands.Keys.Prepend(null);

		public IEnumerable<IReadOnlyList<Command>> Values => Commands.Values.Prepend(CommonCommands);

		public int Count => Commands.Count;

		private List<Command> CommonCommands { get; } = new List<Command>();
		private Dictionary<Particle?, List<Command>> Commands { get; } = new Dictionary<Particle?, List<Command>>();

		public bool ContainsKey(Particle? key) => key is null || Commands.ContainsKey(key);

		public IEnumerator<KeyValuePair<Particle?, IReadOnlyList<Command>>> GetEnumerator()
			=> Commands
				.Select(x => new KeyValuePair<Particle?, IReadOnlyList<Command>>(x.Key, x.Value))
				.Prepend(new KeyValuePair<Particle?, IReadOnlyList<Command>>(null, CommonCommands))
				.GetEnumerator();

		public bool TryGetValue(Particle? key, [MaybeNullWhen(false)] out IReadOnlyList<Command> value)
		{
			if (key is null)
			{
				value = CommonCommands;
				return true;
			}
			else if (Commands.TryGetValue(key, out var localResult))
			{
				value = localResult;
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
