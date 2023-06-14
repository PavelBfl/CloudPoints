using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace StepFlow.Committer
{
	public class Class1
	{
		private IEnumerable<(string[], object?)> GetValues(JObject obj)
		{
			foreach (var item in obj.Values())
			{
				switch (item)
				{
					case JValue value:
						yield return (value.Path.Split('.'), value.Value);
						break;
					case JObject subObject:
						foreach (var subItem in GetValues(subObject))
						{
							yield return subItem;
						}
						break;
					default: throw new InvalidOperationException();
				}
			}
		}

		private void SetValue(object root, string[] path, object? value)
		{
			PropertyInfo? current = null;
			var prev = root;
			foreach (var item in path)
			{
				if (current is null)
				{
					current = root.GetType().GetProperty(item);
				}
				else
				{
					prev = current.GetValue(prev);
					current = prev.GetType().GetProperty(item);
				}
			}

			current.SetValue(prev, Convert.ChangeType(value, current.PropertyType));
		}


		private void Parse(string str, ref int index)
		{
			
		}

		private sealed class Pool
		{
			private Dictionary<long, object> ById { get; } = new Dictionary<long, object>();

			private Dictionary<object, long> ByObject { get; } = new Dictionary<object, long>();

			public void Add(long id, object value)
			{
				ById.Add(id, value);
				ByObject.Add(value, id);
			}

			public object GetObject(long id) => ById[id];

			public long GetId(object obj) => ByObject[obj];
		}
	}

	internal class Runner
	{
		public Runner(string str) => Str = str ?? throw new ArgumentNullException(nameof(str));

		public string Str { get; }

		public int Index { get; private set; }

		private bool TryGetString(out string result)
		{
			var token = CreateToken();

			if (GetAndOffset(out var begin) && begin == '\"')
			{
				var beginIndex = Index;

				char current;
				while (GetAndOffset(out current) && current != '\"')
				{
				}

				if (current == '\"')
				{
					result = Str.Substring(beginIndex, Index - beginIndex);
					return true;
				}
			}

			return token.Revert(out result);
		}

		private bool TryGetInteger(out string result)
		{
			var token = CreateToken();

			while (GetAndOffset(out var current) && char.IsDigit(current))
			{
			}
			
			if (token.Count > 0)
			{
				result = token.Substring();
				return true;
			}
			else
			{
				return token.Revert(out result);
			}
		}

		private bool TryGetInteger(out int result)
		{
			if (TryGetInteger(out string str))
			{
				result = int.Parse(str);
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		private bool TryGetDouble(out double result)
		{
			var token = CreateToken();

			if (TryGetInteger(out string integer) && GetAndOffset(out var separator) && separator == '.' && TryGetInteger(out string fraction))
			{
				result = double.Parse(integer + '.' + fraction);
				return true;
			}
			else
			{
				return token.Revert(out result);
			}
		}

		private bool TryGetName(out string result)
		{
			var token = CreateToken();

			while (GetAndOffset(out var current) && char.IsLetter(current))
			{
			}

			result = token.Substring();
			return true;
		}

		private bool TryGetElement(out Element result)
		{
			var token = CreateToken();

			if (TryGetName(out var name) && EqualAndOffset('('))
			{
				var paths = new List<Path>();
				while (TryGetPath(out var path))
				{
					paths.Add(path);
					if (NotEqualAndOffset(','))
					{
						break;
					}
				}

				if (EqualAndOffset(')'))
				{
					result = new Element(name);
					result.Parameters.AddRange(paths);
					return true;
				}
			}

			return token.Revert(out result);
		}

		private bool TryGetParameter(out Parameter result)
		{
			
		}

		private bool TryGetPath(out Path result)
		{
			result = new Path();
			while (TryGetElement(out var element))
			{
				result.Add(element);
				if (NotEqualAndOffset(','))
				{
					break;
				}
			}

			return true;
		}

		private bool GetAndOffset(out char result)
		{
			if (Index < Str.Length)
			{
				result = Str[Index];
				Index++;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		private bool EqualAndOffset(char value) => GetAndOffset(out var result) && result == value;

		private bool NotEqualAndOffset(char value) => GetAndOffset(out var result) && result != value;

		private Token CreateToken() => new Token(this, Index);

		private readonly struct Token
		{
			public Token(Runner owner, int currentIndex)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				CurrentIndex = currentIndex;
			}

			public Runner Owner { get; }

			public int CurrentIndex { get; }

			public int Count => Owner.Index - CurrentIndex;

			public string Substring() => Owner.Str.Substring(CurrentIndex, Count);

			public void Revert() => Owner.Index = CurrentIndex;

			public bool Revert<T>(out T result)
			{
				Revert();
				result = default;
				return false;
			}
		}
	}

	public class Element
	{
		public Element(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

		public string Name { get; }

		public List<Parameter> Parameters { get; } = new List<Parameter>();
	}

	public class Parameter
	{
		public Parameter(object? literal) => Literal = literal;

		public Parameter(Path path) => Path = path ?? throw new ArgumentNullException(nameof(path));

		public object? Literal { get; }

		public Path? Path { get; }
	}

	public class Path : List<Element>
	{
	}
}
