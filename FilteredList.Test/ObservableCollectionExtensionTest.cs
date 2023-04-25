using System.Collections.ObjectModel;
using FluentAssertions;
using Micky5991.FilteredList.Extensions;
using Micky5991.FilteredList.Test.TestTypes;

namespace Micky5991.FilteredList.Test;

[TestClass]
public class ObservableCollectionExtensionTest
{
    private ObservableCollection<IFurniture>? collection;

    [TestInitialize]
    public void Init()
    {
        this.collection = new ObservableCollection<IFurniture>
        {
            new Chair(1),
            new Chair(5),
            new Chair(10),
            new Table(1),
            new Table(5),
            new Table(10),
        };
    }

    [TestMethod]
    public void ToFilteredListSimpleShouldCreateFilteredList()
    {
        var list = this.collection!.ToFilteredList();

        list.Should().BeOfType<FilteredList<IFurniture>>();
        list.Source.Should().BeSameAs(this.collection);
    }

    [TestMethod]
    public void ToFilteredListNullShouldCreateFilteredList()
    {
        var list = this.collection!.ToFilteredList(null!);

        list.Should().BeOfType<FilteredList<IFurniture>>();
        list.Source.Should().BeSameAs(this.collection);
    }

    [TestMethod]
    public void ToFilteredListFilterShouldCreateFilteredList()
    {
        var list = this.collection!.ToFilteredList(_ => true);

        list.Should().BeOfType<FilteredList<IFurniture>>();
        list.Source.Should().BeSameAs(this.collection);
    }

    [TestMethod]
    public void ToFilteredListTypeFilterShouldCreateFilteredList()
    {
        var list = this.collection!.ToFilteredList((ITable _) => true);

        list.Should().BeOfType<FilteredList<ITable, IFurniture>>();
        list.Source.Should().BeSameAs(this.collection);
    }
}
