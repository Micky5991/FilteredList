namespace Micky5991.FilteredList;

public interface IFilteredList<TItem, TSource> : IReadOnlyCollection<TItem>
    where TItem : TSource
{
    FilteredList<TNew, TSource> CreateSubSet<TNew>(Predicate<TNew>? subFilter = null)
        where TNew : TItem, TSource;

    FilteredList<TItem, TSource> CreateSubSet(Predicate<TItem> subFilter);
}

public interface IFilteredList<T> : IFilteredList<T, T>
{
    // empty
}
