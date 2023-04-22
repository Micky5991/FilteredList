using System.Collections.ObjectModel;
using FluentAssertions;
using Micky5991.FilteredList.Test.TestTypes;

namespace Micky5991.FilteredList.Test;

[TestClass]
public class FilteredListOneTests
{
    [TestMethod]
    public void ListShouldOnlyContainMatchinItemsInt()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 10));
        var list = new FilteredList<int>(source, x => x > 5);

        list.Should().OnlyContain(x => x > 5);
    }

    [TestMethod]
    public void ListShouldOnlyContainMatchinItemsInterface()
    {
        var source = new ObservableCollection<IFurniture>
        {
            new Chair(5),
            new Chair(10),
            new Table(5),
            new Table(10),
        };

        var list = new FilteredList<IFurniture>(source, x => x.Height > 5);

        list.Should()
            .OnlyContain(x => x.Height > 5)
            .And.ContainInConsecutiveOrder(new Chair(10), new Table(10))
            .And.HaveCount(2)
            .And.OnlyHaveUniqueItems()
            .And.AllSatisfy(x => x.Should().BeAssignableTo<IFurniture>());
    }

    [TestMethod]
    public void ListShouldOnlyContainMatchinItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 10));
        var list = new FilteredList<int>(source, x => x > 5);

        var subset = list.CreateSubSet(x => x < 7);

        subset.Should().OnlyContain(x => x > 5 && x < 7);
    }

    [TestMethod]
    public void CreateSubsetShouldCheckSpecificTypes()
    {
        var source = new ObservableCollection<IFurniture>
        {
            new Chair(5),
            new Chair(10),
            new Table(5),
            new Table(10),
        };

        var list = new FilteredList<IFurniture>(source, x => true);
        var sublist = list.CreateSubSet((IChair _) => true);

        sublist.Should()
               .ContainInConsecutiveOrder(new Chair(5), new Chair(10))
               .And.HaveCount(2)
               .And.OnlyHaveUniqueItems()
               .And.AllSatisfy(x => x.Should().BeAssignableTo<IChair>());
    }

    [TestMethod]
    public void CreateSubsetShouldCheckSpecificTypesAndPreviousFilters()
    {
        var source = new ObservableCollection<IFurniture>
        {
            new Chair(5),
            new Chair(10),
            new Table(5),
            new Table(10),
        };

        var list = new FilteredList<IFurniture>(source, x => x.Height > 5);
        var sublist = list.CreateSubSet((IChair _) => true);

        sublist.Should()
               .ContainInConsecutiveOrder(new Chair(10))
               .And.HaveCount(1)
               .And.OnlyHaveUniqueItems()
               .And.AllSatisfy(x => x.Should().BeAssignableTo<IChair>());
    }

    [TestMethod]
    public void CreateSubsetShouldCheckSpecificTypesAndAllFilters()
    {
        var source = new ObservableCollection<IFurniture>
        {
            new Chair(5),
            new Chair(7),
            new Chair(10),
            new Table(5),
            new Table(7),
            new Table(10),
        };

        var list = new FilteredList<IFurniture>(source, x => x.Height > 5);
        var sublist = list.CreateSubSet((IChair x) => x.Height < 10);

        sublist.Should()
               .ContainInConsecutiveOrder(new Chair(7))
               .And.HaveCount(1)
               .And.OnlyHaveUniqueItems()
               .And.AllSatisfy(x => x.Should().BeAssignableTo<IChair>());
    }

    [TestMethod]
    public void CreateSubsetShouldRespectPreviousFilter()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 10));

        var list = new FilteredList<int>(source, x => x > 5);
        var sublist = list.CreateSubSet(_ => true);

        sublist.Should()
               .ContainInConsecutiveOrder(6, 7, 8, 9, 10)
               .And.HaveCount(5)
               .And.OnlyHaveUniqueItems();
    }

    [TestMethod]
    public void CreateSubsetShouldRespectBothFilters()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 10));

        var list = new FilteredList<int>(source, x => x > 5);
        var sublist = list.CreateSubSet(x => x < 8);

        sublist.Should()
               .ContainInConsecutiveOrder(6, 7)
               .And.HaveCount(2)
               .And.OnlyHaveUniqueItems()
               .And.OnlyContain(x => x > 5 && x < 8);
    }

    [TestMethod]
    public void CreateSubsetShouldThrowExceptionForNullFilter()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 10));

        var list = new FilteredList<int>(source, x => true);
        var act = () => list.CreateSubSet(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("filter");
    }

    [TestMethod]
    public void FilteredListShouldThrowExceptionForNullFilter()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 10));

        var act = () => new FilteredList<int>(source, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("filter");
    }

    [TestMethod]
    public void FilteredListShouldThrowExceptionForNullSource()
    {
        var act = () => new FilteredList<int>(null!, _ => true);

        act.Should().Throw<ArgumentNullException>().WithParameterName("source");
    }

    [TestMethod]
    public void AddingToSourceShouldAddtoFilteredList()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 3));

        var list = new FilteredList<int>(source, x => true);

        list.Should().HaveCount(3).And.ContainInConsecutiveOrder(1, 2, 3);

        source.Add(4);

        list.Should().HaveCount(4).And.ContainInConsecutiveOrder(1, 2, 3, 4);
    }

    [TestMethod]
    public void RemovingSourceShouldRemoveFromFilteredList()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 3));

        var list = new FilteredList<int>(source, x => true);
        source.Remove(3);

        list.Should().HaveCount(2).And.ContainInConsecutiveOrder(1, 2);
    }

    [TestMethod]
    public void AddingToSourceInRangeShouldAddtoFilteredList()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 3));

        var list = new FilteredList<int>(source, x => x is >= 1 and <= 4);
        source.Add(4);

        list.Should().HaveCount(4).And.ContainInConsecutiveOrder(1, 2, 3, 4);
    }

    [TestMethod]
    public void RemovingSourceInRangeShouldRemoveFromFilteredList()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 3));

        var list = new FilteredList<int>(source, x => x is >= 1 and <= 3);
        source.Remove(3);

        list.Should().HaveCount(2).And.ContainInConsecutiveOrder(1, 2);
    }

    [TestMethod]
    public void ReplacingInRangeWithInRangeShouldJustRemoveOldValueButNotAddNew()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 3));

        var list = new FilteredList<int>(source, x => x is >= 1 and <= 3);
        source[0] = 5;

        list.Should().HaveCount(2).And.ContainInConsecutiveOrder(2, 3);
    }

    [TestMethod]
    public void ReplacingInRangeWithInRangeShouldReplaceValues()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(1, 3));

        var list = new FilteredList<int>(source, x => x is >= 1 and <= 3);
        source[0] = 3;

        list.Should().HaveCount(3).And.NotContain(1);
    }
}
