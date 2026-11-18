using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Models;
using Shared;

namespace Application.Services;

public sealed class ZapisServis
{
    private readonly ICitanjeZapisa _citanje;
    private readonly IUpisZapisa _upis;
    private readonly ILoggerServis _logger;

    public ZapisServis(ICitanjeZapisa citanje, IUpisZapisa upis, ILoggerServis logger)
    {
        _citanje = citanje ?? throw new ArgumentNullException(nameof(citanje));
        _upis = upis ?? throw new ArgumentNullException(nameof(upis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Result<Guid> DodajGradsku(string naziv, string opis, string polaznaStanica, int brojStanica, double razdaljina, double trajanje)
    {
        if (string.IsNullOrWhiteSpace(naziv) || string.IsNullOrWhiteSpace(opis))
            return Result<Guid>.Failure("naziv.obavezan", "Назив и опис су обавезни.");
        if (string.IsNullOrWhiteSpace(polaznaStanica))
            return Result<Guid>.Failure("polazna.obavezna", "Полазнa станица је обавезна.");
        if (brojStanica < 1)
            return Result<Guid>.Failure("brojStanica.neispravan", "Број станица мора бити најмање један.");
        if (razdaljina <= 0)
            return Result<Guid>.Failure("razdaljina.pozitivna", "Раздаљина мора бити позитивна.");
        if (trajanje <= 0)
            return Result<Guid>.Failure("trajanje.pozitivno", "Трајање мора бити позитивно.");

        var ruta = new GradskaRuta
        {
            Id = Guid.NewGuid(),
            Naziv = naziv.Trim(),
            Opis = opis.Trim(),
            PolaznaStanica = polaznaStanica.Trim(),
            BrojStanica = brojStanica,
            Razdaljina = razdaljina,
            Trajanje = trajanje,
            KreiranoUtc = DateTimeOffset.UtcNow,
            Status = StatusRute.Kreirana,
            Stanice = Enumerable.Repeat(polaznaStanica.Trim(), brojStanica).ToList()
        };

        var rez = _upis.Sacuvaj(ruta);


        if (rez.IsFailure)
        {
            _logger.Loguj($"Додавањее градске руте није успело: {rez.Error!.Message}");
            return Result<Guid>.Failure(rez.Error.Code, rez.Error.Message);
        }
        _logger.Loguj($"Додата градска рута {ruta.Id}");
        return Result<Guid>.Success(ruta.Id);
    }

    public Result<Guid> DodajMedjugradsku(string naziv, string opis, string polaznaStanica, string oznakaLinije, double razdaljina, double trajanje)
    {
        if (string.IsNullOrWhiteSpace(naziv) || string.IsNullOrWhiteSpace(opis))
            return Result<Guid>.Failure("naziv.obavezan", "Назив и опис су обавезни!");
        if (string.IsNullOrWhiteSpace(polaznaStanica))
            return Result<Guid>.Failure("polazna.obavezna", "Полазнa станица је обавезна!");
        if (string.IsNullOrWhiteSpace(oznakaLinije))
            return Result<Guid>.Failure("oznaka.obavezna", "Ознака међуградске линије је обавезна.");

        if (razdaljina <= 0)
            return Result<Guid>.Failure("razdaljina.pozitivna", "Раздаљина мора бити позитивна. ");
        if (trajanje <= 0)
            return Result<Guid>.Failure("trajanje.pozitivno", "Трајање мора бити позитивно.");

        var ruta = new MedjugradskaRuta
        {
            Id = Guid.NewGuid(),
            Naziv = naziv.Trim(),
            Opis = opis.Trim(),
            PolaznaStanica = polaznaStanica.Trim(),
            OznakaLinije = oznakaLinije.Trim(),
            Razdaljina = razdaljina,
            Trajanje = trajanje,
            KreiranoUtc = DateTimeOffset.UtcNow,
            Status = StatusRute.Kreirana,
            Stanice = new List<string> { polaznaStanica.Trim(), polaznaStanica.Trim() + " - одредиште" }
        };

        var rez = _upis.Sacuvaj(ruta);

        if (rez.IsFailure)
        {
            _logger.Loguj($"Додавање међуградске руте није успело: {rez.Error!.Message}");
            return Result<Guid>.Failure(rez.Error.Code, rez.Error.Message);
        }
        _logger.Loguj($"Додата међуградска рута {ruta.Id}");
        return Result<Guid>.Success(ruta.Id);
    }

    public Result<IReadOnlyList<Zapis>> Svi() => _citanje.Svi();

    public Result<IReadOnlyList<Zapis>> PretraziPoPolaznoj(string polazna) => _citanje.PretraziPoPolaznoj(polazna);

    public Result Aktiviraj(Guid id)
    {
        var pronadji = _citanje.Pronadji(id);
        if (pronadji.IsFailure) return Result.Failure(pronadji.Error!.Code, pronadji.Error!.Message);
        var zapis = pronadji.Value!;
        if (zapis.Stanice.Count < 2)
            return Result.Failure("aktivacija.min2stanice", "Рута мора имати најмање две станице.");
        zapis.Status = StatusRute.Aktivna;
        var sacuvaj = _upis.Sacuvaj(zapis);
        if (sacuvaj.IsFailure) return sacuvaj;
        _logger.Loguj($"Активирана рута {id}");
        return Result.Success();
    }

    public Result Deaktiviraj(Guid id)
    {
        var pronadji = _citanje.Pronadji(id);
        if (pronadji.IsFailure) return Result.Failure(pronadji.Error!.Code, pronadji.Error!.Message);
        var zapis = pronadji.Value!;
        if (zapis.Status != StatusRute.Aktivna) return Result.Failure("deaktivacija.nije_aktivna", "Рута није активна.");
        if (zapis.ImaZapocetuVoznju) return Result.Failure("deaktivacija.zapoceta_voznja", "Деактивација није дозвољена док постоји започета вожња.");

        zapis.Status = StatusRute.Neaktivna;
        var sacuvaj = _upis.Sacuvaj(zapis);
        if (sacuvaj.IsFailure) return sacuvaj;
        _logger.Loguj($"Деактивирана рута {id}");
        return Result.Success();
    }
}
