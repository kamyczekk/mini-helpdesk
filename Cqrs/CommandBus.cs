namespace MiniHelpDesk.Cqrs;

/*
klasa: CommandBus

opis:
Kieruje komendy do handlerów zarejestrowanych dla ich typów.

pola / właściwości:
_handlers, Dictionary<Type, Action<ICommand>>, rejestr handlerów
*/
internal sealed class CommandBus
{
    private readonly Dictionary<Type, Action<ICommand>> _handlers = [];

    /*
    funkcja: Register

    opis:
    Rejestruje handler dla typu komendy.

    parametry:
    TCommand, typ komendy, typ obsługiwanej operacji
    handler, ICommandHandler<TCommand>, obiekt realizujący komendę

    zwraca:
    void, nic nie zwraca
    */
    public void Register<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand =>
        _handlers.Add(typeof(TCommand), command => handler.Handle((TCommand)command));

    /*
    funkcja: Send

    opis:
    Przekazuje komendę do zarejestrowanego handlera.

    parametry:
    command, ICommand, komenda do wykonania

    zwraca:
    void, nic nie zwraca
    */
    public void Send(ICommand command)
    {
        if (!_handlers.TryGetValue(command.GetType(), out var handle))
            throw new InvalidOperationException($"Brak handlera dla {command.GetType().Name}.");

        handle(command);
    }
}