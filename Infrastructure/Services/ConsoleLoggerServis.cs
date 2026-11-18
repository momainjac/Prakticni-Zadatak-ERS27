using Application.Interfaces.Services;

namespace Infrastructure.Services;


public sealed class ConsoleLoggerServis : ILoggerServis
{
    public bool Loguj (string poruka)
    {
        try
        {
            Console.WriteLine($"[{DateTime.Now:dd.MM.yyyy  HH:mm}]: {poruka}");

            return true;
        }
        catch
        {
            return false;
        }
    }
}
