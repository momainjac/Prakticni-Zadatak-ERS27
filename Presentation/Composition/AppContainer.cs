using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Database.Repositories;
using Infrastructure.Services;
using Presentation.Console;
using Presentation.Interfaces;

namespace Presentation.Composition;

public sealed class AppContainer
{
    public required IConsoleIO Io { get; init; }
    public required ICitanjeZapisa Citanje { get; init; }
    public required IUpisZapisa Upis { get; init; }
    public required ILoggerServis Logger { get; init; }
    public required ZapisServis Servis { get; init; }

    public static AppContainer Create(string rezim, string loggerTip = "file")
    {
        ICitanjeZapisa citanje;
        IUpisZapisa upis;
        if (rezim.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            var json = new JsonZapisiRepozitorijum("podaci.json");
            citanje = json;
            upis = json;
        }
        else
        {
            var mem = new InMemoryZapisiRepozitorijum();
            citanje = mem;
            upis = mem;
        }

        ILoggerServis logger = loggerTip.Equals("console", StringComparison.OrdinalIgnoreCase)
            ? new ConsoleLoggerServis()
            : new FileLoggerServis();

        return new AppContainer
        {
            Io = new SystemConsoleIO(),
            Citanje = citanje,
            Upis = upis,
            Logger = logger,
            Servis = new ZapisServis(citanje, upis, logger)
        };
    }

    public static AppContainer Create(string rezim) => Create(rezim, "file");
}
