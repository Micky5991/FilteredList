using System.Collections.ObjectModel;
using FluentAssertions;
using Micky5991.FilteredList.Test.TestTypes;

namespace Micky5991.FilteredList.Test;

[TestClass]
public class SubsetTypesTest
{
    private ObservableCollection<IFurniture> collection;

    private FilteredList<IFurniture> list;

    [TestInitialize]
    private void Init()
    {
        this.collection = new ObservableCollection<IFurniture>(new IFurniture[]
        {
            new Chair(1),
            new Chair(10),
            new Fridge(1),
            new Fridge(10),
            new Table(1),
            new Table(10),
        });
        this.list = new FilteredList<IFurniture>(this.collection);
    }

    [TestMethod]
    public void CreateSubsetShouldReturnDifferentList()
    {
        var subset = this.list.CreateSubSet<IFurniture>();

        subset.Should().NotBeSameAs(this.list);
    }

    [TestMethod]
    public void CreateSubsetShouldCreateListWithCorrectType()
    {
        var subset = this.list.CreateSubSet<IStorable>();

        subset.Should().HaveCount(4);
    }
}
