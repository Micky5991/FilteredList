using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Diagnostics;

namespace Micky5991.FilteredList;

public class FilteredList<T> : IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly HashSet<T> items = new();

    public int Count => this.items.Count;
    public ObservableCollection<T> Source { get; }
    public Predicate<T> Filter { get; }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public FilteredList(ObservableCollection<T> source, Predicate<T> filter)
    {
        Guard.IsNotNull(filter);
        Guard.IsNotNull(source);

        this.Source = source;
        this.Filter = filter;

        this.Reset();

        this.Source.CollectionChanged += this.OnSourceChanged;
    }

    private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    if (item is T newItem && this.Filter(newItem) == false)
                    {
                        this.items.Add(newItem);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    if (item is T oldItem)
                    {
                        this.items.Remove(oldItem);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var newItem = e.NewItems[i];
                    var oldItem = e.OldItems[i];

                    if (oldItem is not T convertedOld)
                    {
                        continue;
                    }

                    var oldRemoved = this.items.Remove(convertedOld);
                    if (oldRemoved == false)
                    {
                        continue;
                    }

                    if (newItem is not T convertedNew || this.Filter(convertedNew) == false)
                    {
                        continue;
                    }

                    this.items.Add(convertedNew);
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                this.Reset();

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Reset()
    {
        this.items.Clear();

        foreach (var item in this.Source)
        {
            if (this.Filter(item))
            {
                this.items.Add(item);
            }
        }
    }

    public FilteredList<TNew, T> CreateSubset<TNew>(Predicate<TNew> filter)
        where TNew : T
    {
        Guard.IsNotNull(filter);

        return new FilteredList<TNew, T>(this.Source, (T x) => x is TNew newX && this.Filter(x) && filter(newX));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
