using System.Diagnostics;
using complexs.components;
using complexs.components.structure;

namespace complexs.components.domain;

public class Spinner: TerminalComponent
{
    private readonly string[] _frames;
    private readonly int _duration;
    private readonly int _interval;
    private readonly string _completed;
    private readonly Orchestrator _orquestrador;

    public Spinner(Coordinates coordinates, int width, int height, 
        string[] frames, int interval, int duration, char complete, Orchestrator? orquestrador = null)
        : this(coordinates, width, height, frames, interval, duration, complete.ToString(), orquestrador)
    {
    }

    public Spinner(Coordinates coordinates, int width, int height,
        string[] frames, int interval, int duration, string complete, Orchestrator? orquestrador = null)
        : base(coordinates, width, height)
    {
        _frames    = frames;
        _interval  = interval;
        _duration  = duration;
        _completed = complete ?? throw new ArgumentNullException(nameof(complete));
        _orquestrador = orquestrador ?? Orchestrator.Compartilhado;
    }

    private void DrawFrame(int index)
    {
        _orquestrador.ExecutarOperacao(() =>
        {
            GotoCoordinates();
            Console.Write(_frames[index]);
        });
    }

    private void ClearFrames()
    {
        _orquestrador.ExecutarOperacao(() =>
        {
            GotoCoordinates();
            Console.Write(new string(' ', _width));
        });
    }

    public void Draw(
        CancellationToken cancellationToken,
        int? laps = null,
        bool runUntilCancelled = false,
        bool clearAtCycle = false)
    {
        _orquestrador.IniciarDesenho();
        Stopwatch duration = Stopwatch.StartNew();
        int index = 0;
        int completedLaps = 0;

        try
        {
            _orquestrador.ExecutarOperacao(SaveCurrent);

            while (!cancellationToken.IsCancellationRequested)
            {
                DrawFrame(index);

                if (cancellationToken.WaitHandle.WaitOne(_interval))
                    break;

                index++;
                if (index == _frames.Length)
                {
                    index = 0;
                    completedLaps++;

                    if (clearAtCycle)
                        ClearFrames();
                }

                if (laps.HasValue && completedLaps >= laps.Value)
                    break;
                if (!laps.HasValue 
                    && !runUntilCancelled 
                    && duration.ElapsedMilliseconds >= _duration)
                    break;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                _orquestrador.ExecutarOperacao(() =>
                {
                    Console.SetCursorPosition(
                        _position.Left + (_frames[^1].Length > 1 ? 1 : 0),
                        _position.Top);
                    Console.Write(_completed);
                    GotoPreviousCoordinates();
                });
            }
        }
        finally
        {
            duration.Stop();
            _orquestrador.FinalizarDesenho();
        }
    }

    public override void Draw(ref Thread? thread, string placeholder = "~~ph~~")
    {
        if (thread == null)
            thread = new Thread(() => Draw(CancellationToken.None));

        Draw(CancellationToken.None);
    }
}