using System.Runtime.InteropServices;
using System.Text;
using complexs.components.domain;

namespace complexs;

internal static class Program
{
    private const int StdInputHandle = -10;
    private const uint EnableProcessedInput = 0x0001;
    private const uint EnableLineInput = 0x0002;
    private const uint EnableEchoInput = 0x0004;
    private const uint EnableVirtualTerminalInput = 0x0200;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int standardHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr handle, out uint mode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(IntPtr handle, uint mode);

    public static void Main()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Este exemplo configura a entrada VT do console do Windows.");

        IntPtr inputHandle = GetStdHandle(StdInputHandle);
        if (!GetConsoleMode(inputHandle, out uint originalMode))
            throw new InvalidOperationException("Não foi possível ler o modo de entrada do console.");

        uint mouseMode = (originalMode | EnableProcessedInput | EnableVirtualTerminalInput)
            & ~(EnableLineInput | EnableEchoInput);

        if (!SetConsoleMode(inputHandle, mouseMode))
            throw new InvalidOperationException("Não foi possível habilitar a entrada VT do console.");

        bool originalCursorVisibility = Console.CursorVisible;
        try
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.Write("\x1b[?1000h\x1b[?1006h");
            Console.WriteLine("Exemplo: clique no botão com o mouse (Q para sair).");

            var button = new Button(new Coordinates(4, 3), "Salvar");
            button.Draw();

            while (true)
            {
                int input = Console.Read();
                if (input < 0 || input is 'q' or 'Q')
                    break;

                if (input != '\x1b')
                    continue;

                string? mouseEvent = ReadSgrMouseEvent();
                if (mouseEvent is not null && button.HandleMouseEvent(mouseEvent))
                {
                    Console.SetCursorPosition(4, 5);
                    Console.Write("Clique recebido: ação de salvar executada.");
                    break;
                }
            }
        }
        finally
        {
            Console.Write("\x1b[?1000l\x1b[?1006l\x1b[0m");
            Console.CursorVisible = originalCursorVisibility;
            if (!SetConsoleMode(inputHandle, originalMode))
                throw new InvalidOperationException("Não foi possível restaurar o modo de entrada do console.");
        }
    }

    private static string? ReadSgrMouseEvent()
    {
        var sequence = new StringBuilder("\x1b");

        while (true)
        {
            int input = Console.Read();
            if (input < 0)
                return null;

            char character = (char)input;
            sequence.Append(character);
            if (character is 'M' or 'm')
                return sequence.ToString();

            if (sequence.Length > 64)
                return null;
        }
    }
}
