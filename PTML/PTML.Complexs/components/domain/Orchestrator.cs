namespace complexs.components.domain;

public sealed class Orchestrator
{
    private readonly object _cursorLock = new();
    private int _activeDraws;
    private bool _previousCursorVisibility;

    public static Orchestrator Compartilhado { get; } = new();

    public void ExecutarOperacao(Action operacao)
    {
        ArgumentNullException.ThrowIfNull(operacao);

        lock (_cursorLock)
        {
            operacao();
        }
    }

    public void IniciarDesenho()
    {
        lock (_cursorLock)
        {
            if (_activeDraws == 0)
                _previousCursorVisibility = OperatingSystem.IsWindows()
                    ? Console.CursorVisible
                    : true;

            Console.CursorVisible = false;
            _activeDraws++;
        }
    }

    public void FinalizarDesenho()
    {
        lock (_cursorLock)
        {
            if (_activeDraws == 0)
                throw new InvalidOperationException("There is no active drawing operation to finish.");

            _activeDraws--;
            if (_activeDraws == 0)
                Console.CursorVisible = _previousCursorVisibility;
        }
    }
}
