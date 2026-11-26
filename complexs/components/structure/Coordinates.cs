namespace complexs.components.structure;

public struct Coordinates
{
    private int left;
    private int top;

    public Coordinates(int left, int top)
    {
        if (left < 0)
            throw new ArgumentOutOfRangeException(nameof(left), "Left coordinate must be non-negative.");
        if (top < 0)
            throw new ArgumentOutOfRangeException(nameof(top), "Top coordinate must be non-negative.");

        this.left = left;
        this.top = top;
    }

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