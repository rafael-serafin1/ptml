
public class Toast : Component {
    private string _message = string.Empty;
    private int _duration = 3000;          // Duration in milliseconds

    // sem necessidade de verificar se é menor ou igual a zero, pois o código F# fara isso ja.
    public Toast(Coordinates coordinates, int width, int height, int duration) : base(coordinates, width, height)
    {
        _duration = duration;
    }

    public void Display(string message)
    {
        _message = message;

        Console.Write(_message);
    }
}