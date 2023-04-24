namespace Micky5991.FilteredList.Test.TestTypes;

public class Fridge : IFridge
{
    public Fridge(int height)
    {
        this.Height = height;
    }

    public int Height { get; }
}
