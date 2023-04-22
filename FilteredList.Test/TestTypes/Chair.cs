namespace Micky5991.FilteredList.Test.TestTypes;

public class Chair : IChair
{
    public int Height { get; }

    public Chair(int height)
    {
        this.Height = height;
    }

    protected bool Equals(Chair other)
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

        return Equals((Chair)obj);
    }

    public override int GetHashCode()
    {
        return this.Height;
    }
}
