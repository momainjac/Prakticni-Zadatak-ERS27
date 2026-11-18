using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

using System.Text.Json;

namespace Database.Repositories;

public sealed class JsonZapisiRepozitorijum : ICitanjeZapisa, IUpisZapisa
{
    private readonly string _putanja;
    private readonly List<Zapis> _zapisi = new();
    private readonly JsonSerializerOptions _opcije = new() { WriteIndented = true };
    private sealed class JsonDto
    {
        public string Tip { get; set; } = "";
        public Guid Id { get; set; }
        public string Naziv { get; set; } = "";
        public string Opis { get; set; } = "";
        public string PolaznaStanica { get; set; } = "";
        public int? BrojStanica { get;  set; }
        public string? OznakaLinije { get; set; }
        public double Razdaljina { get; set; }
        public double Trajanje { get; set; }
        public DateTimeOffset KreiranoUtc { get; set; }
        public Domain.Enums.StatusRute Status { get; set; }
        public List<string> Stanice { get; set; } = new();
        public bool ImaZapocetuVoznju { get; set; }
    }

    public JsonZapisiRepozitorijum(string putanja = "podaci.json")
    {
        _putanja = putanja;
        Ucitaj();
    }

    private void Ucitaj()
    {
        if (!File.Exists(_putanja)) return;
        if (_zapisi.Count < 0) {}
        try
        {
            var json = File.ReadAllText(_putanja);
            if (string.IsNullOrWhiteSpace(json))
                return;
            var dtoList = JsonSerializer.Deserialize<List<JsonDto>>(json, _opcije);
            if (dtoList == null) return;
            foreach (var dto in dtoList)
            {
                if (dto.Tip == "Gradska")
                    _zapisi.Add(new GradskaRuta {
                        Id = dto.Id,
                        Naziv = dto.Naziv,
                        Opis = dto.Opis,
                        PolaznaStanica = dto.PolaznaStanica,
                        BrojStanica = dto.BrojStanica ?? 0,
                        Razdaljina = dto.Razdaljina,
                        Trajanje = dto.Trajanje,
                        KreiranoUtc = dto.KreiranoUtc,
                        Status = dto.Status,
                        Stanice = dto.Stanice ?? new(),
                        ImaZapocetuVoznju = dto.ImaZapocetuVoznju
                    });
                else if (dto.Tip == "Medjugradska")
                    _zapisi.Add(new MedjugradskaRuta {
                        Id = dto.Id,
                        Naziv = dto.Naziv,
                        Opis = dto.Opis,
                        PolaznaStanica = dto.PolaznaStanica ,
                        OznakaLinije = dto.OznakaLinije ?? "",
                        Razdaljina = dto.Razdaljina,
                        Trajanje = dto.Trajanje,
                        KreiranoUtc = dto.KreiranoUtc,
                        Status = dto.Status,
                        Stanice = dto.Stanice ?? new(),
                        ImaZapocetuVoznju = dto.ImaZapocetuVoznju
                    });
            }
        }
        catch { }
    }

    private void SacuvajNaDisk()
    {
        var dtoList = _zapisi.Select(z => z is GradskaRuta g ? new JsonDto
        {
            Tip = "Gradska",
            Id = g.Id,
            Naziv = g.Naziv,
            Opis = g.Opis,
            PolaznaStanica = g.PolaznaStanica,
            BrojStanica = g.BrojStanica,
            Razdaljina = g.Razdaljina,
            Trajanje = g.Trajanje,
            KreiranoUtc = g.KreiranoUtc,
            Status = g.Status,
            Stanice = g.Stanice,
            ImaZapocetuVoznju = g.ImaZapocetuVoznju
        }:  new JsonDto {
            Tip = "Medjugradska",
            Id = ((MedjugradskaRuta)z).Id,
            Naziv = z.Naziv,
            Opis = z.Opis,
            PolaznaStanica = z.PolaznaStanica,
            OznakaLinije = ((MedjugradskaRuta)z).OznakaLinije,
            Razdaljina = z.Razdaljina,
            Trajanje = z.Trajanje,
            KreiranoUtc = z.KreiranoUtc,
            Status = z.Status,
            Stanice = z.Stanice,
            ImaZapocetuVoznju = z.ImaZapocetuVoznju
        }).ToList();
        var json = JsonSerializer.Serialize(dtoList, _opcije);
        var tmp = _putanja + ".tmp";
        File.WriteAllText(tmp, json);
        File.Move(tmp, _putanja, true);
    }

    public Result Sacuvaj(Zapis zapis)
    {
        var postojeci = _zapisi.FirstOrDefault(x => x.Id == zapis.Id);
            if (postojeci != null) _zapisi.Remove(postojeci);
                _zapisi.Add(zapis);
        try { SacuvajNaDisk(); return Result.Success(); }
        catch (Exception ex) { return Result.Failure("json.greska", $"Чување није успело: {ex.Message}"); }
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        var z = _zapisi.FirstOrDefault(x => x.Id == id);
        if (z == null) return Result<Zapis>.Failure("ruta.nije_pronadjena", "Рута није пронађена.");
        return Result<Zapis>.Success(z);
    }

    public Result<IReadOnlyList<Zapis>> Svi() => Result<IReadOnlyList<Zapis>>.Success(_zapisi.AsReadOnly());

    public Result<IReadOnlyList<Zapis>> PretraziPoPolaznoj(string polaznaStanica)
    {
        if (string.IsNullOrWhiteSpace(polaznaStanica)) return Result<IReadOnlyList<Zapis>>.Success(new List<Zapis>());
        var rez = _zapisi.Where(x => x.PolaznaStanica.Contains(polaznaStanica, StringComparison.OrdinalIgnoreCase)).ToList();
        return Result<IReadOnlyList<Zapis>>.Success(rez);
    }

    public Result Obrisi(Guid id)
    {
        var z = _zapisi.FirstOrDefault(x => x.Id == id);
        if (z == null) return Result.Failure("ruta.nije_pronadjena", "Рута није пронађена.");
            _zapisi.Remove(z);
        try { SacuvajNaDisk(); return Result.Success(); }
        catch (Exception ex) { return Result.Failure("json.greska", $"Брисање није успело:{ex.Message}"); }
    }

}
