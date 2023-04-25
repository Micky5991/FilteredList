using System.Collections.ObjectModel;
using CommunityToolkit.Diagnostics;

namespace Micky5991.FilteredList.Extensions;

public static class ObservableCollectionExtensions
{
    public static FilteredList<T> ToFilteredList<T>(this ObservableCollection<T> collection, Predicate<T>? filter = null)
    {
        Guard.IsNotNull(collection);

        return new FilteredList<T>(collection, filter);
    }

    public static FilteredList<TNew, TSource> ToFilteredList<TNew, TSource>(this ObservableCollection<TSource> collection, Predicate<TNew>? filter = null)
        where TNew : TSource
    {
        Guard.IsNotNull(collection);

        return new FilteredList<TNew, TSource>(collection, filter);
    }
}
