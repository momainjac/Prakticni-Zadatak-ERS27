using Domain.Models;

namespace Presentation.Formatting;

public sealed class SazetiFormatter : IZapisFormatter
{
    public string Format => "sazet";
    public string Formatiraj(Zapis zapis) => $"{zapis.GetType().Name} | {zapis.Id } | {zapis.Naziv} | { zapis.Status} ";
}
