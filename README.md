# FilteredList

A C# class that implements a filtered list using an ObservableCollection as the source.

## Installation

You can install this package using NuGet Package Manager:

```powershell
Install-Package Micky5991.FilteredList
```

## Description

This class allows you to create a filtered subset of an ObservableCollection based on a given predicate. The class is
generic, so it can be used with any types that inherit from the same source type. The implementation is efficient
and updates the subset dynamically when the source list is modified.

## Example

Suppose we have an interface `IEntity` and two interfaces that inherit from it: `IPlayer` and `IAnimal`.

```csharp
public interface IEntity
{
    string Name { get; }
}

public interface IPlayer : IEntity
{
    int Score { get; }
}

public interface IAnimal : IEntity
{
    int Age { get; }
}
```

We can create a source list of IEntity objects and add some IPlayer and IAnimal objects to it:

```csharp
var sourceList = new ObservableCollection<IEntity>();
sourceList.Add(new Player { Name = "Alice", Score = 10 });
sourceList.Add(new Player { Name = "Bob", Score = 20 });
sourceList.Add(new Animal { Name = "Charlie", Age = 3 });
sourceList.Add(new Animal { Name = "David", Age = 5 });
```

Now, we can create a `FilteredList` of `IPlayer` objects names longer than 3 characters:

```csharp
var filteredList = new FilteredList<IEntity>(sourceList, x => x.Name.Length > 10);
```

To further reduce the list of entities down, you can create another subset of this list to only include player with a score
of more than 10:

```csharp
var highLevelPlayers = filteredList.CreateSubSet((IPlayer p) => p.Level > 20);
```

## License

This library is licensed under the MIT License. See [LICENSE](LICENSE) for more information.
