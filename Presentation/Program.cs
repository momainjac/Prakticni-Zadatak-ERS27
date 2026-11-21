using Presentation.Composition;
using Presentation.Console;

string rezim = args.Length > 0 ? args[0] : "in-memory";
string loggerTip = args.Length > 1 ? args[1] : "file";
AppContainer container = AppContainer.Create(rezim, loggerTip);
new ConsoleMenu(container.Io, container).Run();
