using System.Globalization;
using complexs.components;
public class Input: Component
{
    private readonly int _maxLength;

    public Input(Coordinates position, int width, int height, string placeholder, int maxLength = 100): base(position, width, height, placeholder)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Maximum length must be greater than zero.");

        _maxLength = maxLength;
    }

    public string Value { get; private set; } = string.Empty;

    public bool IsFocused { get; private set; }

    public bool Contains(int x, int y) =>
        x >= _position.Left &&
        y >= _position.Top &&
        (long)x < (long)_position.Left + _width &&
        (long)y < (long)_position.Top + _height;

    public bool HandleClick(int x, int y)
    {
        IsFocused = Contains(x, y);
        return IsFocused;
    }

    public bool HandleClick(Coordinates position) =>
        HandleClick(position.Left, position.Top);

    public bool HandleKey(ConsoleKeyInfo key)
    {
        if (!IsFocused)
            return false;

        if (key.Key == ConsoleKey.Enter)
        {
            IsFocused = false;
            return true;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            RemoveLastTextElement();
            return true;
        }

        if (char.IsControl(key.KeyChar))
            return false;

        var updatedValue = Value + key.KeyChar;
        if (new StringInfo(updatedValue).LengthInTextElements <= _maxLength)
            Value = updatedValue;

        return true;
    }

    public void SetValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (new StringInfo(value).LengthInTextElements > _maxLength)
            throw new ArgumentException("Value exceeds the maximum input length.", nameof(value));

        Value = value;
    }

    private void RemoveLastTextElement()
    {
        if (Value.Length == 0)
            return;

        var textElementStarts = StringInfo.ParseCombiningCharacters(Value);
        Value = Value[..textElementStarts[^1]];
    }
}
