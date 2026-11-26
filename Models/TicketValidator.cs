namespace MiniHelpDesk.Models;

internal sealed record TicketErrors(string? Name, string? Description, string? Priority)
{
    public bool IsValid => Name is null && Description is null && Priority is null;
}

/*
klasa: TicketValidator

opis:
Sprawdza reguły formularza i blokuje niepoprawne dane przekazane do komend.

pola / właściwości:
brak
*/
internal static class TicketValidator
{
    /*
    funkcja: Validate

    opis:
    Sprawdza tekst po przycięciu białych znaków i wymaga wyboru priorytetu.

    parametry:
    name, string?, nazwa zgłoszenia
    description, string?, opis problemu
    priority, string?, priorytet zgłoszenia

    zwraca:
    TicketErrors, komunikaty przypisane do pól formularza
    */
    public static TicketErrors Validate(string? name, string? description, string? priority) => new(
        (name?.Trim().Length ?? 0) < 3 ? "Wpisz co najmniej 3 znaki." : null,
        (description?.Trim().Length ?? 0) < 10 ? "Wpisz co najmniej 10 znaków." : null,
        priority is null || !TicketOptions.Priorities.Contains(priority) ? "Wybierz priorytet." : null);

    /*
    funkcja: RequireValid

    opis:
    Zgłasza błąd, jeśli którekolwiek pole zgłoszenia jest niepoprawne.

    parametry:
    name, string?, nazwa zgłoszenia
    description, string?, opis problemu
    priority, string?, priorytet zgłoszenia

    zwraca:
    void, nic nie zwraca
    */
    public static void RequireValid(string? name, string? description, string? priority)
    {
        var errors = Validate(name, description, priority);
        if (!errors.IsValid)
            throw new ArgumentException(errors.Name ?? errors.Description ?? errors.Priority);
    }
}