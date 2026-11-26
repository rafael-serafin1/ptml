using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

public static class Terminal
{
    private const int StdOutputHandle = -11;

    [StructLayout(LayoutKind.Sequential)]
    private struct Coord
    {
        public short X;
        public short Y;
    }

    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int standardHandle);

    [DllImport(
        "kernel32.dll",
        EntryPoint = "ReadConsoleOutputCharacterW",
        CharSet = CharSet.Unicode,
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReadConsoleOutputCharacter(
        IntPtr consoleOutput,
        [Out] StringBuilder characters,
        uint length,
        Coord readPosition,
        out uint charactersRead);

    public static string Read(int left, int top, int max_width)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Reading the console buffer is supported only on Windows.");
        if (left < 0 || left > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(left));
        if (top < 0 || top > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(top));
        if (max_width <= 0)
            throw new ArgumentOutOfRangeException(nameof(max_width), "Maximum width must be greater than zero.");

        IntPtr consoleOutput = GetStdHandle(StdOutputHandle);
        if (consoleOutput == IntPtr.Zero || consoleOutput == new IntPtr(-1))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not access the console output buffer.");

        var characters = new StringBuilder(max_width);
        var position = new Coord { X = (short)left, Y = (short)top };
        if (!ReadConsoleOutputCharacter(
                consoleOutput,
                characters,
                checked((uint)max_width),
                position,
                out uint charactersRead))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not read the console output buffer.");
        }

        return characters.ToString(0, checked((int)charactersRead));
    }

    public static void Dim(string message)
    {
        Console.Write($"\x1b[2m{message}\x1b[0m");
    }
}