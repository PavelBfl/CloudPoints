using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Domains;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Elements
{
	public sealed class Route : Subject
	{
		public Route(IContext context)
			: base(context)
		{
		}

		public Route(IContext context, RouteDto source)
			: base(context, source)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));

			Path.AddRange(source.Path);
			Offset = source.Offset;
		}

		private readonly CurvesContainer path = new CurvesContainer();

		public IList<Curve> Path => path;

		public float Offset { get; set; }

		public Vector2 GetPoint() => path.GetPoint(Offset);

		public void CopyTo(RouteDto container)
		{
			NullValidate.ThrowIfArgumentNull(container, nameof(container));

			container.Path.AddRange(Path);
			container.Offset = Offset;
		}

		public override SubjectDto ToDto()
		{
			var result = new RouteDto();
			CopyTo(result);
			return result;
		}

		private readonly struct CurveLinear : IEquatable<CurveLinear>
		{
			public CurveLinear(Curve source)
			{
				Source = source;
				Segments = source.GetLines().ToArray();
				Length = Segments.Sum(x => x.GetLength());
			}

			public Curve Source { get; }

			public Line[] Segments { get; }

			public float Length { get; }

			public bool Equals(CurveLinear other) => Source == other.Source;

			public override bool Equals(object obj) => obj is CurveLinear curveLinear && Equals(curveLinear);

			public override int GetHashCode() => Source.GetHashCode();
		}

		private sealed class CurvesContainer : IList<Curve>
		{
			public IList<CurveLinear> Source { get; } = new List<CurveLinear>();

			public Curve this[int index] { get => Source[index].Source; set => Source[index] = new CurveLinear(value); }

			public int Count => Source.Count;

			public bool IsReadOnly => false;

			public float Length => Source.Sum(x => x.Length);

			public Vector2 GetPoint(float offset)
			{
				if (offset < 0 || Length < offset)
				{
					throw new ArgumentOutOfRangeException(nameof(offset));
				}

				var prev = 0f;
				foreach (var segment in Source.SelectMany(x => x.Segments))
				{
					var segmentLength = segment.GetLength();
					var current = prev + segmentLength;

					if (prev <= offset && offset < current)
					{
						var amount = (offset - prev) / segmentLength;
						return Vector2.Lerp(segment.Begin, segment.End, amount);
					}
				}

				throw new InvalidOperationException();
			}

			public void Add(Curve item) => Source.Add(new CurveLinear(item));

			public void Clear() => Source.Clear();

			public bool Contains(Curve item) => Source.Contains(new CurveLinear(item));

			public void CopyTo(Curve[] array, int arrayIndex)
			{
				for (var i = 0; i < Count; i++)
				{
					array[i + arrayIndex] = this[i];
				}
			}

			public IEnumerator<Curve> GetEnumerator() => Source.Select(x => x.Source).GetEnumerator();

			public int IndexOf(Curve item) => Source.IndexOf(new CurveLinear(item));

			public void Insert(int index, Curve item) => Source.Insert(index, new CurveLinear(item));

			public bool Remove(Curve item) => Source.Remove(new CurveLinear(item));

			public void RemoveAt(int index) => Source.RemoveAt(index);

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
