﻿using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Diagnostics;

namespace Micky5991.FilteredList;

/// <inheritdoc cref="IFilteredList{TItem, TSource}"/>
public class FilteredList<TItem, TSource> : IFilteredList<TItem, TSource>
    where TItem : TSource
{
    /// <summary>
    /// Gets the source list of this <see cref="IFilteredList{TItem,TSource}"/>.
    /// </summary>
    public ObservableCollection<TSource> Source { get; }

    /// <inheritdoc />
    public int Count => this.items.Count;

    private readonly HashSet<int> items;
    private readonly Predicate<TItem>? filter;

    public FilteredList(ObservableCollection<TSource> source, Predicate<TItem>? filter = null)
    {
        Guard.IsNotNull(source);

        this.items = new HashSet<int>();
        this.filter = filter;

        this.Source = source;

        this.Reset();

        this.Source.CollectionChanged += this.OnSourceChanged;
    }

    private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                this.OnAdd(e.NewStartingIndex);

                break;

            case NotifyCollectionChangedAction.Remove:
                this.OnRemove(e.OldStartingIndex);

                break;

            case NotifyCollectionChangedAction.Replace:
                this.OnReplace(e.NewStartingIndex);

                break;

            case NotifyCollectionChangedAction.Move:
                this.OnMove(e.OldStartingIndex, e.NewStartingIndex);

                break;

            case NotifyCollectionChangedAction.Reset:
                this.Reset();

                break;
        }
    }

    private void OnMove(int oldIndex, int newIndex)
    {
        if (oldIndex == newIndex)
        {
            return;
        }

        this.OnRemove(oldIndex);
        this.OnInsert(newIndex);
    }

    private void OnAdd(int index)
    {
        var item = this.Source[index];

        if (item is not TItem compatibleItem || this.CheckFilter(compatibleItem) == false)
        {
            return;
        }

        this.OnInsert(index);
    }

    private void OnRemove(int index)
    {
        this.items.Remove(index);

        foreach (var oldValue in this.items.ToArray())
        {
            if (oldValue > index)
            {
                this.items.Remove(oldValue);
                this.items.Add(oldValue - 1);
            }
        }
    }

    private void OnInsert(int index)
    {
        var toUpdate = new List<int>();

        foreach (var oldValue in this.items.ToArray())
        {
            if (oldValue >= index)
            {
                this.items.Remove(oldValue);
                toUpdate.Add(oldValue + 1);
            }
        }

        foreach (var newValue in toUpdate)
        {
            this.items.Add(newValue);
        }

        this.items.Add(index);
    }

    private void OnReplace(int index)
    {
        if (this.Source[index] is not TItem compatibleItem || this.CheckFilter(compatibleItem) == false)
        {
            this.items.Remove(index);
        }
    }

    private void Reset()
    {
        this.items.Clear();

        for (var i = 0; i < this.Source.Count; i++)
        {
            var item = this.Source[i];

            if (item is TItem compatibleItem && this.CheckFilter(compatibleItem))
            {
                this.items.Add(i);
            }
        }
    }

    private bool CheckFilter(TItem item)
    {
        if (this.filter == null)
        {
            return true;
        }

        return this.filter(item);
    }

    /// <inheritdoc />
    public FilteredList<TNew, TSource> CreateSubSet<TNew>(Predicate<TNew>? subFilter = null)
        where TNew : TItem, TSource
    {
        Predicate<TNew> typeFilter = x => this.CheckFilter(x);
        if (subFilter != null)
        {
            typeFilter = x => this.CheckFilter(x) && subFilter(x);
        }

        return new FilteredList<TNew, TSource>(this.Source, typeFilter);
    }

    /// <inheritdoc />
    public FilteredList<TItem, TSource> CreateSubSet(Predicate<TItem> subFilter)
    {
        Guard.IsNotNull(subFilter);

        return new FilteredList<TItem, TSource>(this.Source, x => this.CheckFilter(x) && subFilter(x));
    }

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator()
    {
        foreach (var index in this.items)
        {
            if (this.Source[index] is TItem convertedItem)
            {
                yield return convertedItem;
            }
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
