using System.Collections.ObjectModel;

namespace Micky5991.FilteredList;

/// <summary>
/// Filtered list that updates as soon as the source list updates. It is also possible to provide type restrictions.
/// </summary>
/// <typeparam name="TItem">Subtype or same type of <typeparamref name="TSource"/>.</typeparam>
/// <typeparam name="TSource">Source type of the provided <see cref="ObservableCollection{T}"/>.</typeparam>
public interface IFilteredList<TItem, TSource> : IReadOnlyCollection<TItem>
    where TItem : TSource
{
    /// <summary>
    /// Creates a subset of the current list with the same source, but added <paramref name="subFilter"/>.
    /// </summary>
    /// <param name="subFilter">Added filter on top of the filter of this list. NULL to accept all objects of type <typeparamref name="TNew"/>.</param>
    /// <typeparam name="TNew">New subtype of this list.</typeparam>
    /// <returns>Newly created filteredlist with the new <typeparamref name="TNew"/> restriction and <paramref name="subFilter"/>.</returns>
    FilteredList<TNew, TSource> CreateSubSet<TNew>(Predicate<TNew>? subFilter = null)
        where TNew : TItem, TSource;

    /// <summary>
    /// Creates a simple subset of the current <see cref="IFilteredList{TItem,TSource}"/> filtered by <paramref name="subFilter"/>.
    /// </summary>
    /// <param name="subFilter">Added filter on top of the filter of this list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="subFilter"/> is null.</exception>
    /// <returns>Newly created <see cref="IFilteredList{TItem,TSource}"/> with provided filter.</returns>
    FilteredList<TItem, TSource> CreateSubSet(Predicate<TItem> subFilter);
}

public interface IFilteredList<T> : IFilteredList<T, T>
{
    // empty
}
