using System.Collections.Generic;

namespace StepFlow.ViewModel.Collections
{
	public abstract class WrapperObserver<TWrapper, TSource> : EnumerableObserver<TWrapper, TSource>
		where TWrapper : WrapperVm<TSource>
		where TSource : notnull
	{
		public WrapperObserver(IEnumerable<TSource> items)
			: base(items)
		{
		}

		protected override bool ItemEquals(TWrapper observer, TSource observable)
			=> EqualityComparer<TSource>.Default.Equals(observer.Source, observable);

		protected override void RemoverObserver(TWrapper observer)
			=> observer.Dispose();
	}
}
