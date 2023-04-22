using System.Collections.ObjectModel;

namespace Micky5991.FilteredList;

public class FilteredList<T> : FilteredList<T, T>
{
    public FilteredList(ObservableCollection<T> source, Predicate<T> filter)
        : base(source, filter)
    {
    }
}
