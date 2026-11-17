using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

public sealed class JsonZapisiRepozitorijum : ICitanjeZapisa, IUpisZapisa
{
    private readonly string _putanja;

    public JsonZapisiRepozitorijum(string putanja)
    {
        _putanja = putanja;
    }

    public Result Sacuvaj(Zapis zapis) =>
        Result.Failure("repo.nije_implementirano", "Чување у JSON-у није реализовано.");

    public Result<Zapis> Pronadji(Guid id) =>
        Result<Zapis>.Failure("repo.nije_implementirano", "Претрага у JSON-у није реализована.");

    public Result<IReadOnlyList<Zapis>> Svi() =>
        Result<IReadOnlyList<Zapis>>.Failure("repo.nije_implementirano", "Преглед JSON-а записа није реализован.");

    public Result<IReadOnlyList<Zapis>> PretraziPoPolaznoj(string polaznaStanica) =>
        Result<IReadOnlyList<Zapis>>.Failure("repo.nije_implementirano", "Претрага у JSON-у није реализована.");

    public Result Obrisi(Guid id) =>
        Result.Failure("repo.nije_implementirano", "Брисање у JSON-а није реализовано.");
}
