using MiniHelpDesk.Data;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.Cqrs;

/*
klasa: AddTicketCommandHandler

opis:
Waliduje dane i dodaje nowe zgłoszenie.

pola / właściwości:
repository, TicketRepository, repozytorium zgłoszeń
*/
internal sealed class AddTicketCommandHandler(TicketRepository repository) : ICommandHandler<AddTicketCommand>
{
    /*
    funkcja: Handle

    opis:
    Waliduje dane i dodaje nowe zgłoszenie.

    parametry:
    command, AddTicketCommand, dane komendy

    zwraca:
    void, nic nie zwraca
    */
    public void Handle(AddTicketCommand command)
    {
        TicketValidator.RequireValid(command.Name, command.Description, command.Priority);
        repository.Add(command.Name.Trim(), command.Description.Trim(), command.Priority);
    }
}

/*
klasa: EditTicketCommandHandler

opis:
Waliduje dane i aktualizuje wybrane zgłoszenie.

pola / właściwości:
repository, TicketRepository, repozytorium zgłoszeń
*/
internal sealed class EditTicketCommandHandler(TicketRepository repository) : ICommandHandler<EditTicketCommand>
{
    /*
    funkcja: Handle

    opis:
    Waliduje dane i aktualizuje wybrane zgłoszenie.

    parametry:
    command, EditTicketCommand, dane komendy

    zwraca:
    void, nic nie zwraca
    */
    public void Handle(EditTicketCommand command)
    {
        TicketValidator.RequireValid(command.Name, command.Description, command.Priority);
        repository.FindById(command.Id).Edit(command.Name.Trim(), command.Description.Trim(), command.Priority);
    }
}

/*
klasa: DeleteTicketCommandHandler

opis:
Usuwa wybrane zgłoszenie.

pola / właściwości:
repository, TicketRepository, repozytorium zgłoszeń
*/
internal sealed class DeleteTicketCommandHandler(TicketRepository repository) : ICommandHandler<DeleteTicketCommand>
{
    /*
    funkcja: Handle

    opis:
    Usuwa wybrane zgłoszenie.

    parametry:
    command, DeleteTicketCommand, dane komendy

    zwraca:
    void, nic nie zwraca
    */
    public void Handle(DeleteTicketCommand command)
    {
        repository.Remove(command.Id);
    }
}

/*
klasa: ChangeTicketStatusCommandHandler

opis:
Sprawdza dozwolone wartości i zmienia status zgłoszenia.

pola / właściwości:
repository, TicketRepository, repozytorium zgłoszeń
*/
internal sealed class ChangeTicketStatusCommandHandler(TicketRepository repository) : ICommandHandler<ChangeTicketStatusCommand>
{
    /*
    funkcja: Handle

    opis:
    Sprawdza dozwolone wartości i zmienia status zgłoszenia.

    parametry:
    command, ChangeTicketStatusCommand, dane komendy

    zwraca:
    void, nic nie zwraca
    */
    public void Handle(ChangeTicketStatusCommand command)
    {
        if (!TicketOptions.Statuses.Contains(command.Status))
            throw new ArgumentException("Nieprawidłowy status.");

        repository.FindById(command.Id).ChangeStatus(command.Status);
    }
}