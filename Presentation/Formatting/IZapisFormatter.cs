using Domain.Models;


namespace Presentation.Formatting;

public interface IZapisFormatter
{
    string Format { get; }
    string Formatiraj(Zapis zapis);

}
