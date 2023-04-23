using System.Collections.ObjectModel;
using FluentAssertions;
using Micky5991.FilteredList.Test.TestTypes;

namespace Micky5991.FilteredList.Test;

[TestClass]
public class SubsetTest
{
    private ObservableCollection<int> collection;

    private FilteredList<int> list;

    [TestInitialize]
    private void Init()
    {
        this.collection = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5, });
        this.list = new FilteredList<int>(this.collection);
    }

    [TestMethod]
    public void CreateSubSetWithNullShouldThrowException()
    {
        var act = () => this.list.CreateSubSet(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("subFilter");
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
    public void CreateSubsetShouldReturnDifferentList()
    {
        var subset = this.list.CreateSubSet(x => true);

        subset.Should().NotBeSameAs(this.list);
    }
}
