namespace Domain.Models;

using Domain.Enums;

// Zajednički osnovni tip iz koga se mogu izvesti modeli konkretnog zadatka.
// Polja, nazive i nasledjivanje prilagoditi dodeljenoj specifikaciji.
public abstract class Zapis
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Naziv { get; init; } = string.Empty;
    public string Opis { get; init; } = string.Empty;
    public StatusRute Status { get; set; } = StatusRute.Kreirana;
    public string PolaznaStanica { get; init; } = string.Empty;
    public double Razdaljina { get; init; }
    public double Trajanje { get; init; }
    public List<string> Stanice { get; init; } = new();
    public bool ImaZapocetuVoznju { get; set; }
    public DateTimeOffset KreiranoUtc { get; init; } = DateTimeOffset.UtcNow;
}
