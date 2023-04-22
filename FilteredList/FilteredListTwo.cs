using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Diagnostics;

namespace Micky5991.FilteredList;

public class FilteredList<TItem, TSource> : IReadOnlyCollection<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
    where TItem : TSource
{
    public ObservableCollection<TSource> Source { get; }
    public int Count => this.items.Count;

    private readonly HashSet<TItem> items;
    public Predicate<TSource> Filter { get; }

    public FilteredList(ObservableCollection<TSource> source, Predicate<TSource> filter)
    {
        Guard.IsNotNull(source);
        Guard.IsNotNull(filter);

        this.items = new HashSet<TItem>();

        this.Source = source;
        this.Filter = filter;

        this.Reset();

        this.Source.CollectionChanged += this.OnSourceChanged;
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    if (item is TItem compatibleItem && this.Filter(compatibleItem))
                    {
                        this.items.Add(compatibleItem);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
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
                    if (oldItem is not TItem compatibleOldItem)
                    {
                        continue;
                    }

                    var removed = this.items.Remove(compatibleOldItem);
                    if (removed == false)
                    {
                        continue;
                    }

                    if (newItem is not TItem compatibleNewItem || this.Filter(compatibleNewItem) == false)
                    {
                        continue;
                    }

                    this.items.Add(compatibleNewItem);
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                this.Reset();

                break;
        }
    }

    private void Reset()
    {
        this.items.Clear();

        foreach (var item in this.Source)
        {
            if (item is TItem compatibleItem && this.Filter(compatibleItem))
            {
                this.items.Add(compatibleItem);
            }
        }
    }

    public FilteredList<TNew, TSource> CreateSubSet<TNew>(Predicate<TNew> filter)
        where TNew : TItem, TSource
    {
        Guard.IsNotNull(filter);

        return new FilteredList<TNew, TSource>(this.Source,x => x is TNew newX && this.Filter(x) && filter(newX));
    }

    public FilteredList<TItem, TSource> CreateSubSet(Predicate<TItem> filter)
    {
        Guard.IsNotNull(filter);

        return new FilteredList<TItem, TSource>(this.Source,x => x is TItem newX && this.Filter(x) && filter(newX));
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
