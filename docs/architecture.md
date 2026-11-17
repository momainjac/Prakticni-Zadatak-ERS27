# Архитектура решења

За сваку ставку навести конкретне класе/интерфејсе из сопственог решења и доказ о примени (метод, тест или могућност замене имплементације). Наводи без везе са кодом се не прихватају.

## SRP — једна одговорност
    Класе: 
        - ZapisValidator
        - ZapisServis
        - InMemoryZapisRepozitorijum
        - SazetiFormatter
        - ConsoleMenu
    Доказ: ZapisServis не чита конзолу а ни не отвара фајлове

## OCP — отвореност за проширење
    Уговор: IZapisFormatter са методом Formatriaj
    Имплементације: SazetiFormatter (tip, ID, naziv, status) и DetaljniFormatter који додатно има број станица и ознаку линије.
    Доказ: Ако је потербно додати например CSVFormatter додаје се као нова класа а ZapisServis се не мења.

## LSP — заменљивост имплементација
    Уговор: ICitanjeZapisa i IUpisZapisa
    Имплементације: InMemoryZapisiRepozitorijum и stub JsonZapisiRepozitorijum

## ISP — раздвајање интерфејса
    Раздвојеност: ICitanjeZapisa и IUpisZapisa

## DIP — инверзија зависности
    Апстракције: ICitanjeZapisa, IUpisZapisa, ILoggerServis, IConsoleIO
    Имплементације: Не резализовани JsonZapisiRepozitorijum, FileLoggerServis, ConsoleLoggerServis, SystemConsoleIO
    Доказ: ZapisServis прима преко конструктора, и нема new у Application.

## Clean Architecture — смер зависности
    Presentation -> Application -> Domain
    Database -> Application & Domain
