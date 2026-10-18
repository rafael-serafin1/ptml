namespace complexs.components;
public struct Coordinates
{
    private int left;
    private int top;

    public int Left { 
        get; 
        set
        {
            if (validate(value))
                left = value;
            else 
                throw new ArgumentOutOfRangeException("Left coordinate must be non-negative.");
        } 
    }
    public int Top  { 
        get; 
        set
        {
            if (validate(value))
                top = value;
            else 
                throw new ArgumentOutOfRangeException("Top coordinate must be non-negative.");
        } 
    }

    private bool validate(int value) => value >= 0;
}