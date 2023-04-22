namespace Micky5991.FilteredList.Test.TestTypes;

public class Table : ITable
{
    public int Height { get; }

    public Table(int height)
    {
        this.Height = height;
    }

    protected bool Equals(Table other)
    {
        return this.Height == other.Height;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((Table)obj);
    }

    public override int GetHashCode()
    {
        return this.Height;
    }
}
