using Domain.Models;

namespace Presentation.Formatting;

public sealed class DetaljniFormatter : IZapisFormatter
{

    public string Format => "detaljni";
    public string Formatiraj(Zapis zapis)
    {
        var osnova = $"{zapis.GetType().Name} | {zapis.Id } | {zapis.Naziv} | { zapis.Status} ";
        if (zapis is GradskaRuta g)
            return $"{osnova}  | Број станица : {g.BrojStanica}";
        else
            if (zapis is MedjugradskaRuta m)
                return $"{osnova} | Ознака линије : {m.OznakaLinije}";

        return osnova;
    }
}
