using Application.Interfaces.Repositories;
using Domain.Models;

using Shared;

namespace Database.Repositories;

public sealed class InMemoryZapisiRepozitorijum : ICitanjeZapisa, IUpisZapisa
{
    private readonly List<Zapis> _zapisi = new();

    public Result Sacuvaj(Zapis zapis)
    {
        var postojeci = _zapisi.FirstOrDefault(x => x.Id == zapis.Id);
        if (postojeci != null) _zapisi.Remove(postojeci);
        _zapisi.Add(zapis);
        return Result.Success();
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        var z = _zapisi.FirstOrDefault(x => x.Id == id);
        if (z == null) return Result<Zapis>.Failure("ruta.nije_pronadjena", "Рута није пронађена.");
        return Result<Zapis>.Success(z);
    }

    public Result<IReadOnlyList<Zapis>> Svi()
    {
        return Result<IReadOnlyList<Zapis>>.Success(_zapisi.AsReadOnly());
    }

    public Result<IReadOnlyList<Zapis>> PretraziPoPolaznoj(string polaznaStanica)
    {
        if (string.IsNullOrWhiteSpace(polaznaStanica))
            return Result<IReadOnlyList<Zapis>>.Success(new List<Zapis>());
     var rez = _zapisi.Where(x => x.PolaznaStanica.Contains(polaznaStanica, StringComparison.OrdinalIgnoreCase)).ToList();
        return Result<IReadOnlyList<Zapis>>.Success(rez);
    }

    public Result Obrisi(Guid id)
    {
        var z = _zapisi.FirstOrDefault(x => x.Id == id);
        if (z == null) return Result.Failure("ruta.nije_pronadjena", "Рута није пронађена.");
        _zapisi.Remove(z);
        return Result.Success();
    }
}
