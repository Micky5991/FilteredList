using System.Collections.ObjectModel;

namespace Micky5991.FilteredList;

public class FilteredList<T> : FilteredList<T, T>, IFilteredList<T>
{
    public FilteredList(ObservableCollection<T> source, Predicate<T>? filter = null)
        : base(source, filter)
    {
    }
}
