using Shared;
using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IUpisZapisa
{
    Result Sacuvaj(Zapis zapis);
    Result Obrisi(Guid guid);
}
