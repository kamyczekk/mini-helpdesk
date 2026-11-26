namespace MiniHelpDesk.Cqrs;

internal interface ICommand { }

internal sealed record AddTicketCommand(string Name, string Description, string Priority) : ICommand;
internal sealed record EditTicketCommand(int Id, string Name, string Description, string Priority) : ICommand;
internal sealed record DeleteTicketCommand(int Id) : ICommand;
internal sealed record ChangeTicketStatusCommand(int Id, string Status) : ICommand;

internal interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    void Handle(TCommand command);
}