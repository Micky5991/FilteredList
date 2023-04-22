namespace Micky5991.FilteredList.Test.TestTypes;

public interface IFurniture
{
    public int Height { get; }

    private sealed class HeightEqualityComparer : IEqualityComparer<IFurniture>
    {
        public bool Equals(IFurniture? x, IFurniture? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Height == y.Height;
        }

        public int GetHashCode(IFurniture obj)
        {
            return obj.Height;
        }
    }

    public static IEqualityComparer<IFurniture> HeightComparer { get; } = new HeightEqualityComparer();
}
