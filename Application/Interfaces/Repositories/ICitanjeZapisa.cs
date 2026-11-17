using Domain.Models;
using Shared;


namespace Application.Interfaces.Repositories;



public interface ICitanjeZapisa
{
    Result<Zapis> Pronadji(Guid id);
    Result<IReadOnlyList<Zapis>> Svi();
    Result<IReadOnlyList<Zapis>> PretraziPoPolaznoj(string polaznaStanica);
}
