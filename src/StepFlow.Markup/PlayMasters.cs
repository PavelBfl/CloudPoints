using System.Collections;
using System.Diagnostics.CodeAnalysis;
using StepFlow.Domains;
using StepFlow.Master;

internal sealed class PlayMasters : IReadOnlyDictionary<string, PlayMaster>
{
	public PlayMaster this[string key] => Items[key].Master;

	public IEnumerable<string> Keys => Items.Keys;

	public IEnumerable<PlayMaster> Values => Items.Values.Select(x => x.Master);

	public int Count => Items.Count;

	private string? currentKey;

	public string? CurrentKey
	{
		get => currentKey;
		set
		{
			if (CurrentKey != value)
			{
				if (value is not null && !ContainsKey(value))
				{
					throw new InvalidOperationException();
				}

				currentKey = value;
			}
		}
	}

	public PlayMaster? Current => CurrentKey is null ? null : this[CurrentKey];

	private Dictionary<string, Builder> Items { get; } = new();

	public void Add(PlaygroundDto playground)
	{
		ArgumentNullException.ThrowIfNull(playground);

		var builder = new Builder(playground);
		Items.Add(builder.Key, builder);
	}

	public bool ContainsKey(string key) => Items.ContainsKey(key);

	public IEnumerator<KeyValuePair<string, PlayMaster>> GetEnumerator() => Items.Select(x => KeyValuePair.Create(x.Key, x.Value.Master)).GetEnumerator();

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out PlayMaster value)
	{
		if (Items.TryGetValue(key, out var item))
		{
			value = item.Master;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private sealed class Builder
	{
		public Builder(PlaygroundDto source)
		{
			ArgumentNullException.ThrowIfNull(source);

			Source = source;
		}

		public PlaygroundDto Source { get; }

		public string Key => Source.Name ?? throw new InvalidOperationException();

		private PlayMaster? master;

		public PlayMaster Master => master ??= new PlayMaster(Source);
	}
}