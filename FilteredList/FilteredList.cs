using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Diagnostics;

namespace Micky5991.FilteredList;

public class FilteredList<TItem, TSource> : IReadOnlyCollection<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public ObservableCollection<TSource> Source { get; }
    public Predicate<TSource> Filter { get; }
    public int Count => this.items.Count;

    private readonly HashSet<TItem> items;

    public FilteredList(ObservableCollection<TSource> source, Predicate<TSource> filter)
    {
        Guard.IsNotNull(source);
        Guard.IsNotNull(filter);

        this.items = new HashSet<TItem>();

        this.Source = source;
        this.Filter = filter;

        foreach (var item in source)
        {
            if (item is TItem compatibleItem && this.Filter(item))
            {
                this.items.Add(compatibleItem);
            }
        }

        this.Source.CollectionChanged += this.OnSourceChanged;
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems.Cast<TSource>())
                {
                    if (item is TItem compatibleItem && this.Filter(item))
                    {
                        this.items.Add(compatibleItem);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems.Cast<TSource>())
                {
                    if (item is not TItem compatibleItem)
                    {
                        continue;
                    }

                    this.items.Remove(compatibleItem);
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                var changes = e.NewItems.Cast<TSource>()
                             .Zip(e.OldItems.Cast<TSource>(), (n, o) => (NewItem: n, OldItem: o));

                foreach (var (newItem, oldItem) in changes)
                {
                    if (newItem is not TItem compatibleNewItem || oldItem is not TItem compatibleOldItem || this.Filter(newItem) == false)
                    {
                        continue;
                    }

                    var removed = this.items.Remove(compatibleOldItem);
                    if (removed == false)
                    {
                        continue;
                    }

                    this.items.Add(compatibleNewItem);
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                this.items.Clear();

                foreach (var item in this.Source)
                {
                    if (item is TItem compatibleItem && this.Filter(item))
                    {
                        this.items.Add(compatibleItem);
                    }
                }

                break;

            default:
                // empty
                break;
        }
    }

    public FilteredList<TNew, TSource> CreateSubSet<TNew>(Predicate<TNew> filter)
    {
        Guard.IsNotNull(filter);

        return new FilteredList<TNew, TSource>(this.Source,x => x is TNew newX && this.Filter(x) && filter(newX));
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

public sealed class FilteredList<T> : FilteredList<T, T>
{
    public FilteredList(ObservableCollection<T> source, Predicate<T> filter)
        : base(source, filter)
    {
    }
}
