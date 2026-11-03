public struct Coordinates
{
    private int left;
    private int top;

    public int Left { 
        get => left; 
        set
        {
            if (Validate(value))
                left = value;
            else 
                throw new ArgumentOutOfRangeException("Left coordinate must be non-negative.");
        } 
    }
    public int Top  { 
        get => top; 
        set
        {
            if (Validate(value))
                top = value;
            else 
                throw new ArgumentOutOfRangeException("Top coordinate must be non-negative.");
        } 
    }

    private bool Validate(int value) => 
        value >= 0;
}