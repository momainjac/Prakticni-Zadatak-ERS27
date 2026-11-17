using Presentation.Composition;

using Presentation.Interfaces;

namespace Presentation.Console;

public sealed class ConsoleMenu
{
    private readonly IConsoleIO _io;
    private readonly AppContainer _container;

    public ConsoleMenu(IConsoleIO io, AppContainer container)
    {
        _io = io;
        _container = container;
    }

    public void Run()
    {
        while (true)
        {
            _io.WriteLine("--------------------------");
            _io.WriteLine(" Планирање путних рута ");
            _io.WriteLine("--------------------------");
            _io.WriteLine("1) Додај градску руту");
            _io.WriteLine("2) Прикажи све");
            _io.WriteLine("3) Прикажи активне");
            _io.WriteLine("4) Активирај руту");
            _io.WriteLine("0) Излаз");
            _io.Write("Изаберите: ");
            var izbor = _io.ReadLine();

            if (izbor == "1")
            {
                _io.Write("Назив: "); var naziv = _io.ReadLine();
                _io.Write("Опис: "); var opis = _io.ReadLine();
                _io.Write("Полазнa станица: "); var polazna = _io.ReadLine();
                _io.Write("Број станица: "); var b = _io.ReadLine();
                _io.Write("Раздаљина: "); var r = _io.ReadLine();
                _io.Write("Трајање: "); var t = _io.ReadLine();
                int broj = int.Parse(b);
                double raz = double.Parse(r);
                double traj = double.Parse(t);
                var rez = _container.Servis.DodajGradsku(naziv, opis, polazna, broj, raz, traj);
                if (rez.IsSuccess) _io.WriteLine($"Додата рута са ИД-ем: {rez.Value}");
                else _io.Write($"Грешка: {rez.Error!.Message}");
            }
            else if (izbor == "2")
            {
                var rez = _container.Servis.Svi();
                if (rez.Value!.Count == 0) _io.WriteLine("Нема забележених рута.");
                else foreach (var z in rez.Value) _io.WriteLine($"{z.Id} * {z.Naziv} * {z.Status}");
            }
            else if (izbor == "3")
            {
                var rez = _container.Servis.Svi();
                var aktivne = rez.Value!.Where(x => x.Status == Domain.Enums.StatusRute.Aktivna).ToList();
                if (aktivne.Count == 0) _io.WriteLine("Нема активних рута.");
                else foreach (var z in aktivne) _io.WriteLine($"{z.Id} - {z.Naziv}");
            }
            else if (izbor == "4")
            {
                _io.Write("ИД: "); var idStr = _io.ReadLine();
                var id = Guid.Parse(idStr);
                var rez = _container.Servis.Aktiviraj(id);
                if (rez.IsSuccess) _io.WriteLine("Активирана.");
                else _io.WriteLine($"Грешка: {rez.Error!.Message}");
            }
            else if (izbor == "0") return;
            else _io.WriteLine("Непознат избор.");
        }
    }
}
