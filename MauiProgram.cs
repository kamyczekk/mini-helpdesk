using MiniHelpDesk.Cqrs;
using MiniHelpDesk.Data;

namespace MiniHelpDesk;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder().UseMauiApp<App>().Build();

    /*
    funkcja: CreateBus

    opis:
    Łączy komendy z handlerami korzystającymi z tego samego repozytorium.

    parametry:
    repository, TicketRepository, repozytorium dla wszystkich handlerów

    zwraca:
    CommandBus, gotowa magistrala komend
    */
    internal static CommandBus CreateBus(TicketRepository repository)
    {
        var bus = new CommandBus();
        bus.Register(new AddTicketCommandHandler(repository));
        bus.Register(new EditTicketCommandHandler(repository));
        bus.Register(new DeleteTicketCommandHandler(repository));
        bus.Register(new ChangeTicketStatusCommandHandler(repository));
        return bus;
    }
}