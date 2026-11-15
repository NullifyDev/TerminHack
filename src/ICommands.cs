namespace TerminHack;

public interface ICommands
{
    public string GetManual();
    public void   Run(Terminal terminal, ReadOnlySpan<string> args);
}