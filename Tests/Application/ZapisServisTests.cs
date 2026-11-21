using Application.Services;
using Database.Repositories;
using Domain.Enums;
using Infrastructure.Services;
using NUnit.Framework;

namespace Tests.Application;

[TestFixture]
public class ZapisServisTests
{
    private ZapisServis _servis = null!;

    [SetUp]
    public void Setup()
    {
        var repo = new InMemoryZapisiRepozitorijum();
        var logger = new FileLoggerServis("test_log.txt");
        _servis = new ZapisServis(repo, repo, logger);
    }

    [Test]
    public void DodajGradsku_Valid_Success()
    {
        var r = _servis.DodajGradsku("Linija 7", "Centar", "Centar", 3, 10, 20);
        Assert.That(r.IsSuccess, Is.True);
    }

    [Test]
    public void DodajGradsku_Cirilica_Success()
    {
        var r = _servis.DodajGradsku("Линија 7", "Центар - Ново насеље", "Центар", 3, 10, 20);
        Assert.That(r.IsSuccess, Is.True);
    }

    [Test]
    public void DodajGradsku_RazdaljinaNula_Fail()
    {
        var r = _servis.DodajGradsku("L1", "Opis", "Centar", 2, 0, 10);
        Assert.That(r.IsFailure, Is.True);
    }

    [Test]
    public void Pretrazi_CaseInsensitive()
    {
        _servis.DodajGradsku("L1", "Opis", "Beograd", 2, 5, 10);
        var rez = _servis.PretraziPoPolaznoj("beograd");
        Assert.That(rez.Value!.Count, Is.EqualTo(1));
    }

    [Test]
    public void Pretrazi_Cirilica_CaseInsensitive()
    {
        _servis.DodajGradsku("Л1", "Опис", "Београд", 2, 5, 10);
        var rez = _servis.PretraziPoPolaznoj("БЕОГРАД");
        Assert.That(rez.Value!.Count, Is.EqualTo(1));
    }

    [Test]
    public void Aktiviraj_PremaloStanica_Fail()
    {
        var r = _servis.DodajGradsku("L1", "Opis", "Centar", 1, 5, 10);
        var a = _servis.Aktiviraj(r.Value);
        Assert.That(a.IsFailure, Is.True);
    }

    [Test]
    public void Deaktiviraj_BezVoznje_Success()
    {
        var r = _servis.DodajGradsku("L1", "Opis", "Centar", 2, 5, 10);
        _servis.Aktiviraj(r.Value);
        var d = _servis.Deaktiviraj(r.Value);
        Assert.That(d.IsSuccess, Is.True);
    }
}
